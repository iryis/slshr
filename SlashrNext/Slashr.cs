using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using SlashrNext.Commands;
using SlashrNext.Events;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext;

public class Slashr : BackgroundService
{
    internal static DiscordClient discordClient = null!;
    internal static Stopwatch uptime = null!;
    public static JsonElement config;
    public static bool isDebug;


    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        if (args.Contains("--debug"))
        {
            isDebug = true;
            Logger.Msg("DEBUG mode enabled.");
        }
#if DEBUG
        Logger.Msg("Running in DEBUG build mode");
#endif

        var configPath = isDebug
            ? $"{Environment.CurrentDirectory}/config.json"
            : $"{Environment.CurrentDirectory}/config/config.json";
        config = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(configPath));
        Moderation.LoadPassported();

        builder.Services.AddHostedService<Slashr>();

        var host = builder.Build();
        await host.RunAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        discordClient = DiscordClientBuilder.CreateDefault(
                config.GetProperty("token").GetString() ?? throw new InvalidOperationException("where token"),
                DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages
            ).ConfigureEventHandlers(e => e.HandleSessionCreated(SessionCreated)
                .HandleZombied(ClientZombied)
                .HandleUnknownEvent(UnknownEvent)
                .HandleGuildCreated(GuildJoined)
                .HandleGuildDownloadCompleted(GuildDownloadComplete)
                .HandleMessageCreated(OnMessage)
                .HandleComponentInteractionCreated(ComponentInteractedWith)
                .HandleGuildMemberAdded(GuildMemberAdded)
                .HandleGuildMemberUpdated(GuildMemberUpdated)
            ).UseCommands(((_, extension) =>
            {
                var textCmdProcessor = new TextCommandProcessor(new TextCommandConfiguration
                {
                    PrefixResolver = new DefaultPrefixResolver(true, "+").ResolvePrefixAsync,
                    EnableCommandNotFoundException = false,
                    IgnoreBots = true
                });
                var slashCmdProcessor = new SlashCommandProcessor(new SlashCommandConfiguration
                {
                    RegisterCommands = true,
                    NamingPolicy = new KebabCaseNamingPolicy(),
                    UnconditionallyOverwriteCommands = true
                });
                extension.AddCommands(Assembly.GetExecutingAssembly());
                extension.AddParameterCheck<MaxStringLengthCheck>();
                extension.AddProcessor(textCmdProcessor);
                extension.AddProcessor(slashCmdProcessor);
            }))
            .Build();

        Logger.Msg("Discord client initialized.");

        var confStatus = config.GetProperty("status");
        var status = Extensions.ParseActivity(confStatus.GetProperty("name").GetString() ?? "nothing",
            confStatus.GetProperty("type").GetString() ?? "Playing");

        await discordClient.ConnectAsync(status);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // shutting down
        }
    }

    private async Task GuildMemberUpdated(DiscordClient client, GuildMemberUpdatedEventArgs args)
    {
        var logChannel = await client.GetChannelAsync("logChannel".GetConfigValue().GetUInt64());
        var memberRole = await args.Guild.GetRoleAsync(731091614453202944);
        if (args is { PendingBefore: true, PendingAfter: false })
        {
            await logChannel.SendMessageAsync($"{args.Member.Mention} has passed membership screening.");
            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(10));
                await args.Member.GrantRoleAsync(memberRole, "Member completed rules screening");
                await logChannel.SendMessageAsync($"{args.Member.Mention} was given member role.");
            });
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await discordClient.DisconnectAsync();
        }
        catch
        {
            // ignore disconnect exceptions during shutdown
        }

        await base.StopAsync(cancellationToken);
    }

    private static async Task SessionCreated(DiscordClient sender, SessionCreatedEventArgs args)
    {
        uptime = Stopwatch.StartNew();
        Logger.Msg(
            $"Session created! Connected as {sender.CurrentUser.Username}#{sender.CurrentUser.Discriminator} ({sender.CurrentUser.Id})");
        await Task.Run(async () =>
        {
            await sender.GetChannelAsync("logChannel".GetConfigValue().GetUInt64()).Result
                .SendMessageAsync("Bot session has connected");
            Abyss.TryCleanup(discordClient);
        });
    }

    private static async Task OnMessage(DiscordClient c, MessageCreatedEventArgs args)
    {
        var slashGuildId = "slashGuildId".GetConfigValue().GetUInt64();

        if (args.Guild.Id != slashGuildId ||
            args.Author is { IsBot: true } or { IsSystem: true } or { IsCurrent: true })
        {
            await HandleAbyssChannel(args);
        }
    }

    private static async Task HandleAbyssChannel(MessageCreatedEventArgs args)
    {
        var abyssChannelId = "abyssChannel".GetConfigValue().GetUInt64();
        if (args.Channel.Id == abyssChannelId)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(2));
                if (args.Message is { Pinned: true }) return;
                try
                {
                    await args.Message.DeleteAsync();
                }
                catch (NotFoundException)
                {
                    Logger.Warn(
                        $"Could not delete abyss message [{args.Message.Id}], maybe it was already deleted?");
                }
            });
        }
    }

    private static Task GuildDownloadComplete(DiscordClient dClient, GuildDownloadCompletedEventArgs args)
    {
        return Task.Run(() =>
        {
            Logger.Msg($"Guild download of {args.Guilds.Count} guilds complete!");
            Logger.Msg("Guild list: ");
            foreach (var (_, guild) in args.Guilds)
            {
                Logger.Msg($"{guild.Name} ({guild.Id}) [{dClient.GetConnectionLatency(guild.Id)}]");
            }
        });
    }

    private static Task GuildJoined(DiscordClient sender, GuildCreatedEventArgs args)
    {
        if (args.Guild.Id == "slashGuildId".GetConfigValue().GetUInt64()) return Task.CompletedTask;
        Logger.Warn($"Non-target guild has been joined - {args.Guild.Name} ({args.Guild.Id})");
        return Task.CompletedTask;
    }

    private async Task ComponentInteractedWith(DiscordClient client, ComponentInteractionCreatedEventArgs args)
    {
        // neko command button handlers
        switch (args.Id)
        {
            case "sl_nk":
                await Fun.HandleMoreButton(args);
                break;
            case "sl_nk_del":
                await args.Message.DeleteAsync();
                break;
        }
    }

    private async Task GuildMemberAdded(DiscordClient client, GuildMemberAddedEventArgs args)
    {
        var gate = "gateEnabled".GetConfigValue().GetBoolean();
        if (!gate || args.Guild.Id != 685617157916459012) return;
        Logger.Msg($"Guild member added: {args.Member.Username} ({args.Member.Id})");
        var user = await client.GetUserAsync(args.Member.Id);
        var time = DateTime.Now - args.Member.CreationTimestamp.DateTime;
        if (time.Days >= 1) return;
        if (Moderation.passported.Contains(args.Member.Id))
        {
            Moderation.passported.Remove(args.Member.Id);
            Moderation.SavePassported();
            return;
        }

        var logChannel = await client.GetChannelAsync("logChannel".GetConfigValue().GetUInt64());
        try
        {
            await args.Member.SendMessageAsync(
                $"Sorry, your Discord account is too new to join {args.Guild.Name} at this time. Please try rejoining after your account is at least one day old. ");
        }
        catch
        {
            // member has dms off
            await logChannel.SendMessageAsync(
                $"Unable to notify {user.Username} of the minimum account age requirement, they likely have direct messages disabled.");
        }

        await args.Member.RemoveAsync($"Account too new ({time.Hours} hours {time.Minutes} minutes)");

        await logChannel.SendMessageAsync(
            $"{user.Username} (#{user.Discriminator}) ({user.Id}) has been kicked: Account is too new");
    }

    // error handlers
    private static Task UnknownEvent(DiscordClient sender, UnknownEventArgs e)
    {
        Logger.Warn($"Ignoring unknown event {e.EventName}");
        return Task.CompletedTask;
    }

    private static Task ClientZombied(DiscordClient sender, ZombiedEventArgs e)
    {
        Logger.Error(
            $"Client has failed {e.Failures} heartbeats (during guild dl: {!e.GuildDownloadCompleted}) and is " +
            $"now a zombie. Killing this process");
        sender.DisconnectAsync();
        sender.Dispose();
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}