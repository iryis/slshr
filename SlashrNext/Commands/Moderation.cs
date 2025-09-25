using System.ComponentModel;
using System.Text.Json;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext.Commands;

public class Moderation
{
    public static List<ulong> passported = [];

    [Command("passport")]
    [Description("Exempt a user from the new member gate")]
    [RequireMod]
    [ServersOnly]
    public async Task Passport(SlashCommandContext ctx, [Description("User to passport")] DiscordUser user)
    {
        await ctx.DeferResponseAsync(true);
        if (ctx.Guild is null || ctx.Member is null)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("This command can only be used in a server.").AsEphemeral());
            return;
        }

        passported.Add(user.Id);
        var resp =
            $"{user.Mention} ({user.Id}) has been exempted from the new member filter. This lasts until this member's next join.";
        await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent(resp).AsEphemeral());
        SavePassported();
        var logChannel = await ctx.Client.GetChannelAsync("logChannel".GetConfigValue().GetUInt64());
        await logChannel.SendMessageAsync(resp);
    }

    [Command("change-status")]
    [Description("Change the bot's status")]
    [RequireMod]
    [ServersOnly]
    public async Task ChangeStatus(SlashCommandContext ctx,
        [Description("The activity type (playing, listening, etc)")] ActivityType at,
        [Description("Status to change to.")] string status)
    {
        await ctx.DeferResponseAsync(true);
        if (ctx.Guild is null || ctx.Member is null)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("This command can only be used in a server.").AsEphemeral());
            return;
        }

        try
        {
            var dType = at switch
            {
                ActivityType.Playing => DiscordActivityType.Playing,
                ActivityType.Streaming => DiscordActivityType.Streaming,
                ActivityType.ListeningTo => DiscordActivityType.ListeningTo,
                ActivityType.Watching => DiscordActivityType.Watching,
                ActivityType.Custom => DiscordActivityType.Custom,
                ActivityType.Competing => DiscordActivityType.Competing,
                _ => DiscordActivityType.Playing
            };
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, dType));
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("Status changed!").AsEphemeral());
        }
        catch (Exception exc)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent(
                $"Failed to change status. \n\n```cs\n{exc.Message}```").AsEphemeral());
        }
    }

    internal static void LoadPassported()
    {
        var h = JsonSerializer.Deserialize<List<ulong>>(
            File.ReadAllText(Slashr.isDebug
                ? $"{"debugFolderPath".GetConfigValue().GetString()}/passported.json"
                : $"{Environment.CurrentDirectory}/config/passported.json"));
        if (h == null) return;
        passported = h;
    }

    internal static void SavePassported()
    {
        var passports = JsonSerializer.Serialize(passported, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Slashr.isDebug
            ? $"{"debugFolderPath".GetConfigValue().GetString()}/passported.json"
            : $"{Environment.CurrentDirectory}/config/passported.json", passports);
    }

    public enum ActivityType
    {
        Playing,
        Streaming,
        ListeningTo,
        Watching,
        Custom,
        Competing
    }
}