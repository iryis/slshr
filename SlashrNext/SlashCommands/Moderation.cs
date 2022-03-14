using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext.SlashCommands;

public class Moderation : ApplicationCommandModule
{
    public static List<ulong> passported = new();

    [SlashCommand("passport", "Exempt a user from the new member gate", false)]
    [SlashRequireMod]
    public async Task Passport(InteractionContext ctx, [Option("user", "User id to passport.")] DiscordUser user)
    {
        passported.Add(user.Id);
        var resp =
            $"{user.Mention} ({user.Id}) has been exempted from the new member filter. This lasts until this member's next join.";
        await ctx.CreateResponseAsync(resp, true);
        SavePassported();
        await ctx.Guild.GetChannel("logChannel".GetConfigValue().GetUInt64()).SendMessageAsync(resp);
    }

    [SlashCommand("change-status", "Change the bot's status", false)]
    [SlashRequireMod]
    public async Task ChangeStatus(InteractionContext ctx,
        [Option("activity_type", "The activity type (playing, listening, etc)")]
        ActivityType at, [Option("status", "Status to change to.")] string status)
    {
        try
        {
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, (DSharpPlus.Entities.ActivityType) at));
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Status changed!").AsEphemeral());
        }
        catch (Exception exc)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Failed to change status. \n\n```cs\n{exc.Message}```"));
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
        var passports = JsonSerializer.Serialize(passported, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(Slashr.isDebug
            ? $"{"debugFolderPath".GetConfigValue().GetString()}/passported.json"
            : $"{Environment.CurrentDirectory}/config/passported.json", passports);
    }

    public enum ActivityType
    {
        [ChoiceName("Playing")] Playing,
        [ChoiceName("Streaming")] Streaming,
        [ChoiceName("Listening to")] ListeningTo,
        [ChoiceName("Watching")] Watching
    }
}