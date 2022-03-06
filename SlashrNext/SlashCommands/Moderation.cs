using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext.SlashCommands;

public class Moderation : ApplicationCommandModule
{
    public static List<ulong> passported = new();

    [SlashCommand("passport", "Exempt a user from the new member gate.")]
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

    internal static void LoadPassported()
    {
        var h = JsonSerializer.Deserialize<List<ulong>>(
            File.ReadAllText(Slashr.isDebug
                ? $"{"debugFolderPath".GetConfigValue().GetString()}/passported.json"
                : $"{Environment.CurrentDirectory}/passported.json"));
        if (h == null) return;
        passported = h;
    }

    internal static void SavePassported()
    {
        var passports = JsonSerializer.Serialize(passported, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(Slashr.isDebug
            ? $"{"debugFolderPath".GetConfigValue().GetString()}/passported.json"
            : $"{Environment.CurrentDirectory}/passported.json", passports);
    }
}