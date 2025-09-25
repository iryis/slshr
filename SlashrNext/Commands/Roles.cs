using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using SlashrNext.Utils;

namespace SlashrNext.Commands;

public class Roles
{
    [Command("irl-role")]
    [Description("Check eligibility for the IRL role")]
    public async Task IrlRole(CommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        if (ctx.Guild is null || ctx.Member is null)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("This command can only be used in a server."));
            return;
        }

        var member = ctx.Member;
        var irlRole = await ctx.Guild.GetRoleAsync(761738544070131732); // 761738544070131732
        if (member.Roles.Contains(irlRole))
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("You already have it!"));
            return;
        }

        var joined = DateTime.Now - member.JoinedAt.DateTime;
        if (joined.Days < 5)
        {
            var tryTime = member.JoinedAt.AddSeconds(432000).ToUnixTimeSeconds();
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"You're not quite there yet! Try again <t:{tryTime.ToString()}:R>"));
            return;
        }

        await ctx.Member.GrantRoleAsync(irlRole, "Member qualified for irl role");
        await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("Done! You have been given the IRL role!"));
    }

    [Command("irl-optout")]
    [Description("Remove yourself from the irl pics channel")]
    public async Task IrlRemove(SlashCommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        if (ctx.Guild is null || ctx.Member is null)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("This command can only be used in a server."));
            return;
        }

        var member = ctx.Member;
        var irlRole = await ctx.Guild.GetRoleAsync(761738544070131732); // 761738544070131732
        if (!member.Roles.Contains(irlRole))
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("You don't even have the role lol"));
            return;
        }
        await ctx.Member.RevokeRoleAsync(irlRole, "Member opt-out of IRL role");
        await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent(
            "You have been removed from the irl role & corresponding channel. If you would like to opt-in again, use `/irl-role` or ask a mod."));
    }

    [Command("in-role")]
    [Description("Show how many members have a specific role")]
    public async Task inRole(CommandContext ctx,
        [Description("The role to get a member count for")]
        DiscordRole role)
    {
        await ctx.DeferResponseAsync();
        if (ctx.Guild is null)
        {
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("This command can only be used in a server."));
            return;
        }

        try
        {
            var members = new List<DiscordMember>();
            await foreach (var m in ctx.Guild.GetAllMembersAsync())
            {
                if (m.Roles.Contains(role)) members.Add(m);
            }

            var embed = new DiscordEmbedBuilder()
                .WithDescription(members.Count == 1
                    ? $"**There is {members.Count} member with {role.Mention}**"
                    : $"**There are {members.Count} members with {role.Mention}**")
                .WithColor(new DiscordColor("#DEDDFF"));
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred fetching role members: {ex}");
        }
    }
}