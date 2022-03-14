#pragma warning disable CS8618
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using SlashrNext.SlashCommands;
using SlashrNext.Utils;

namespace SlashrNext;

public class Slashr
{
    internal static DiscordClient client;
    internal static Stopwatch uptime;
    public static JsonElement config;
    public static bool isDebug;


    private static void Main(string[] args)
    {
        if (args.Contains("--debug"))
        {
            isDebug = true;
            Logger.Msg("Bot Debug Mode Enabled");
        }
#if DEBUG
        Logger.Msg("Running in DEBUG build mode");
#endif

        config = JsonSerializer.Deserialize<JsonElement>(
            File.ReadAllText(isDebug
                ? $"{Environment.CurrentDirectory}/../../../config.json"
                : $"{Environment.CurrentDirectory}/config/config.json"));
        Moderation.LoadPassported();
        MainAsync().GetAwaiter().GetResult();
    }

    public static async Task MainAsync()
    {
        client = new DiscordClient(new DiscordConfiguration
        {
            Token = config.GetProperty("token").GetString(),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages
        });
        Logger.Msg("Bot Started");

        client.Ready += OnReady;
        client.ClientErrored += OnClientError;
        client.SocketErrored += OnSocketError;

        client.GuildCreated += GuildJoined;

        client.GuildDownloadCompleted += GuildDownloadComplete;
        client.MessageCreated += OnMessage;
        client.UnknownEvent += (_, args) =>
        {
            Logger.Msg($"Ignoring unknown event {args.EventName}");
            return Task.CompletedTask;
        };


        var commands = client.UseCommandsNext(new CommandsNextConfiguration
        {
            StringPrefixes = new[] {"+"}, EnableDefaultHelp = false
        });

        commands.CommandErrored += (_, e) =>
        {
            return Task.Run(() =>
                Logger.Error($"Command {e.Command.Name}: {e.Exception}\nContext: {e.Context}"));
        };
        var slash = client.UseSlashCommands();
        slash.SlashCommandErrored += (c, e) =>
        {
            return Task.Run(async () =>
            {
                if (e.Exception is InvalidOperationException)
                {
                    Logger.Msg($"{e.Context.Interaction.Data.Name}");
                    Logger.Msg($"{e.Context.InteractionId}");
                    Logger.Msg($"{e.Context.Interaction.Id}");
                    var co = await c.Client.GetGuildApplicationCommandsAsync(e.Context.Guild.Id);
                    foreach (var discordApplicationCommand in co)
                    {
                        Logger.Msg($"{discordApplicationCommand.Name} {discordApplicationCommand.Id}");
                    }

                    await c.Client.DeleteGuildApplicationCommandAsync(e.Context.Guild.Id, e.Context.Interaction.Id);
                    await c.RefreshCommands();
                    return;
                }

                Logger.Error($"SlashCommand Error: {e.Exception}\nContext: {e.Context}");
            });
        };

        slash.ContextMenuErrored += (_, e) =>
        {
            return Task.Run(() =>
                Logger.Error($"Context Menu Error: {e.Exception}\nContext: {e.Context}"));
        };

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "SlashrNext.Events"))
        {
            type.GetMethod("RegisterEvents")?.Invoke(null, null);
        }

        commands.RegisterCommands(Assembly.GetExecutingAssembly());
        slash.RegisterCommands(Assembly.GetExecutingAssembly(), "slashGuildId".GetConfigValue().GetUInt64());
        var status = config.GetProperty("status");
        await client.ConnectAsync(Extensions.ParseActivity(status.GetProperty("name").GetString() ?? "nothing",
            status.GetProperty("type").GetString() ?? "Playing"));
        await Task.Delay(-1);
    }

    private static async Task OnReady(DiscordClient sender, ReadyEventArgs args)
    {
        uptime = Stopwatch.StartNew();
        Logger.Msg("Bot Ready");
        await Task.Run(async () =>
        {
            await sender.GetChannelAsync("logChannel".GetConfigValue().GetUInt64()).Result
                .SendMessageAsync("Bot is connected");
        });
    }

    private static Task OnMessage(DiscordClient c, MessageCreateEventArgs msg)
    {
        return Task.Run(() =>
        {
            if (msg.Guild == null || msg.Guild != null && msg.Guild.Id == "slashGuildId".GetConfigValue().GetUInt64() ||
                msg.Author.IsBot || msg.Author.IsSystem == true || msg.Author.IsCurrent) return;
            if (msg.Message.Content.Contains("<@363916474127220739>") ||
                msg.Message.Content.Contains("<@!363916474127220739>"))
            {
                msg.Message.RespondAsync("<https://roleypoly.com/s/685617157916459012>");
            }
        });
    }

    private static Task GuildDownloadComplete(DiscordClient dClient, GuildDownloadCompletedEventArgs args)
    {
        return Task.Run(() =>
        {
            Logger.Msg($"Online in {dClient.Guilds.Count} guilds! ({dClient.Ping}ms)");
            foreach (var (_, guild) in dClient.Guilds)
            {
                Console.WriteLine($"{guild.Name} ({guild.Id})");
            }
        });
    }

    private static Task GuildJoined(DiscordClient sender, GuildCreateEventArgs args)
    {
        return Task.Run(() =>
        {
            if (args.Guild.Id == "slashGuildId".GetConfigValue().GetUInt64()) return;
            var channel = args.Guild.SystemChannel ?? args.Guild.GetDefaultChannel();
            try
            {
                channel.SendMessageAsync(
                    "👋 Hello! I am a single-server bot, so my slash commands and some other functions are not available here." +
                    " If you didn't mean to invite me, feel free to kick.");
            }
            catch
            {
                // ignored
            }
        });
    }

    // error handlers
    private static Task OnClientError(DiscordClient sender, ClientErrorEventArgs e)
    {
        return Task.Run(() => Logger.Error($"Client Error thrown by {e.EventName}: {e.Exception}"));
    }

    private static Task OnSocketError(DiscordClient sender, SocketErrorEventArgs e)
    {
        return Task.Run(() => Logger.Error($"Socket Error: {e.Exception}"));
    }
}