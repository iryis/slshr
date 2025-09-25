using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using SlashrNext.Objects;
using SlashrNext.Utils;
using System.ComponentModel;

namespace SlashrNext.Commands;

public class Admin
{
    [Command("shutdown")]
    [RequireMod]
    public async Task Shutdown(TextCommandContext ctx)
    {
        Logger.Log("Received shutdown command, shutting down...");
        await ctx.RespondAsync("Shutting down...");
        await ctx.Client.DisconnectAsync();
        ctx.Client.Dispose();
        Environment.Exit(0);
    }


    [Command("say")]
    [RequireMod]
    public async Task SlashSay(TextCommandContext ctx, [RemainingText] string txt)
    {
        try
        {
            await ctx.Message.DeleteAsync();
        }
        catch
        {
            // ignored, bot has no perms most likely
        }

        await ctx.RespondAsync(txt);
    }

    [Command("sayc")]
    [RequireMod]
    public async Task SlashSay(TextCommandContext ctx, DiscordChannel channel, [RemainingText] string txt)
    {
        try
        {
            await ctx.Message.DeleteAsync();
            await channel.SendMessageAsync(txt);
        }
        catch
        {
            // ignored, bot has no perms most likely
            Logger.Warn("Failed to send message to other channel, bot has no permissions");
        }
    }

    /// <summary>
    /// This can ONLY edit embeds sent by the same bot user. aka you cant edit carl embeds this way
    /// It will also only edit the first embed because im lazy and having functionality for multiple is just tedious to make
    /// Edit an embed's body.
    /// </summary>
    [Command("editembedbody")]
    [RequireMod]
    [ServersOnly]
    public async Task EditEmbed(TextCommandContext ctx, DiscordMessage msg, [RemainingText] string content)
    {
        if (msg.Author is null) return;
        if (msg.Author.Id != ctx.Client.CurrentUser.Id)
        {
            await ctx.RespondAsync("You can only edit embeds sent by the same bot user.");
            return;
        }

        try
        {
            await msg.ModifyAsync(new DiscordEmbedBuilder(msg.Embeds[0]).WithDescription(content).Build());
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Embed body edited."));
        }
        catch (Exception ex)
        {
            // ignored, bot has no perms most likely
            await ctx.RespondAsync("Failed to edit embed.");
            Logger.Warn($"Failed to edit embed body of {msg.Id}:\n{ex}");
        }
    }

    /// <summary>
    /// im doing it this way until i get less lazy
    /// </summary>
    [Command("edittitle")]
    [RequireMod]
    public async Task EditEmbedTitle(TextCommandContext ctx, DiscordMessage msg, [RemainingText] string content)
    {
        if (msg.Author is null) return;
        if (msg.Author.Id != ctx.Client.CurrentUser.Id)
        {
            await ctx.RespondAsync("You can only edit embeds sent by the same bot user.");
            return;
        }

        try
        {
            await msg.ModifyAsync(new DiscordEmbedBuilder(msg.Embeds[0]).WithTitle(content).Build());
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Embed title edited."));
        }
        catch (Exception ex)
        {
            // ignored, bot has no perms most likely
            await ctx.RespondAsync("Failed to edit embed.");
            Logger.Warn($"Failed to edit embed title of {msg.Id}:\n{ex}");
        }
    }

    [Command("slashcmdssuck")]
    [Description("Delete EVERY cmd. discord suck")]
    [RequireMod]
    [ServersOnly]
    public async Task CleanupSlashCommands(CommandContext ctx)
    {
        Logger.Warn("Cleaning up slash cmds!");
        await ctx.DeferResponseAsync();

        try
        {
            var guildId = "slashGuildId".GetConfigValue().GetUInt64();

            var globalCmds = await ctx.Client.GetGlobalApplicationCommandsAsync();
            var guildCmds = await ctx.Client.GetGuildApplicationCommandsAsync(guildId);

            var deletedGlobal = 0;
            var deletedGuild = 0;

            foreach (var cmd in globalCmds)
            {
                await ctx.Client.DeleteGlobalApplicationCommandAsync(cmd.Id);
                deletedGlobal++;
            }

            foreach (var cmd in guildCmds)
            {
                await ctx.Client.DeleteGuildApplicationCommandAsync(guildId, cmd.Id);
                deletedGuild++;
            }

            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Done. Deleted {deletedGlobal} global and {deletedGuild} guild slash command(s). " +
                             $"They will be re-added on the bots next restart")
                .AsEphemeral());
        }
        catch (Exception ex)
        {
            Logger.Error($"Slash cleanup failed: {ex}");
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                .WithContent("Failed to clean slash commands. Check logs for details.")
                .AsEphemeral());
        }
    }
}