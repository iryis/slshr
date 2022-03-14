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
}