using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SlashrNext.Utils;

namespace SlashrNext.Commands;

public class ModOnly : BaseCommandModule
{
    [Command("shutdown"), RequireOwner]
    public async Task Shutdown(CommandContext ctx)
    {
        Logger.Log("Received shutdown command, shutting down...");
        await ctx.RespondAsync("Shutting down...");
        await ctx.Client.DisconnectAsync();
        ctx.Client.Dispose();
        Environment.Exit(0);
    }


    [Command("say"), RequireUserPermissions(Permissions.ManageMessages)]
    public async Task SlashSay(CommandContext ctx, [RemainingText] string txt)
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

    [Command("sayc"), RequireUserPermissions(Permissions.ManageMessages)]
    public async Task SlashSay(CommandContext ctx, DiscordChannel channel, [RemainingText] string txt)
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
    [Command("ebody"), RequireUserPermissions(Permissions.ManageMessages)]
    public async Task EditEmbed(CommandContext ctx, DiscordMessage msg, [RemainingText] string content)
    {
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
    [Command("etitle"), RequireUserPermissions(Permissions.ManageMessages)]
    public async Task EditEmbedTitle(CommandContext ctx, DiscordMessage msg, [RemainingText] string content)
    {
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
    
    [Command("eroles"), RequireUserPermissions(Permissions.ManageMessages)]
    public async Task EditRolesField(CommandContext ctx, DiscordMessage msg, [RemainingText] string content)
    {
        if (msg.Author.Id != ctx.Client.CurrentUser.Id)
        {
            await ctx.RespondAsync("You can only edit embeds sent by the same bot user.");
            return;
        }

        try
        {
            await msg.ModifyAsync(new DiscordEmbedBuilder(msg.Embeds[0]).RemoveFieldAt(0).AddField("Roles", content, true).Build());
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Roles field edited."));
        }
        catch (Exception ex)
        {
            // ignored, bot has no perms most likely
            await ctx.RespondAsync("Failed to edit embed.");
            Logger.Warn($"Failed to edit roles field of {msg.Id}:\n{ex}");
        }
    }
}