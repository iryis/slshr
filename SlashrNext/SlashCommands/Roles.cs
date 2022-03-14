using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SlashrNext.Utils;

namespace SlashrNext.SlashCommands;

public class Roles : ApplicationCommandModule
{
    [SlashCommand("irl-role", "Check eligibility for the IRL role")]
    public async Task IrlRole(InteractionContext ctx)
    {
        var member = ctx.Member;
        var irlRole = ctx.Guild.GetRole(934632958986436668); // 761738544070131732
        if (member.Roles.Contains(irlRole))
        {
            await ctx.CreateResponseAsync("You already have it!");
            return;
        }

        var joined = DateTime.Now - member.JoinedAt.DateTime;
        if (joined.Days < 5)
        {
            var tryTime = member.JoinedAt.AddSeconds(432000).ToUnixTimeSeconds();
            await ctx.CreateResponseAsync($"You're not quite there yet! Try again <t:{tryTime}:R>");
            return;
        }

        await ctx.Member.GrantRoleAsync(irlRole, "Member qualified for irl role");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Done! You have been given the IRL role!"));
    }

    [SlashCommand("in-role", "Show how many members have a specific role")]
    public async Task RoleMembers(InteractionContext ctx,
        [Option("role", "The role to get member count for")]
        DiscordRole role)
    {
        await ctx.DeferAsync(true);
        try
        {
            var members = ctx.Guild.GetAllMembersAsync().GetAwaiter().GetResult()
                .Where(m => m.Roles.Contains(role)).ToList();
            var embed = new DiscordEmbedBuilder()
                .WithDescription(members.Count == 1
                    ? $"**There is {members.Count} member with {role.Mention}**"
                    : $"**There are {members.Count} members with {role.Mention}**").WithColor(role.Color);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred fetching role members: {ex}");
        }
    }
}