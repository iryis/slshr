using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace SlashrNext.Commands;

public class Global
{
    [Command("hello")]
    [Description("Say hi!")]
    public async Task HelloCommand(CommandContext ctx)
    {
        await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Hey {ctx.User.Mention}!"));
    }

    [Command("uptime")]
    [Description("Check the bot's uptime")]
    public async Task BotUptime(CommandContext ctx)
    {
        var time = Slashr.uptime.Elapsed;
        var latency = "\n-# Connection latency only available in servers";
        if (ctx.Guild is not null)
        {
            latency = $"\nConnection latency: {ctx.Client.GetConnectionLatency(ctx.Guild.Id).TotalMilliseconds}ms";
        }
        await ctx.RespondAsync(new DiscordMessageBuilder().WithContent(
            $"Current uptime: {time.Hours:00}:{time.Minutes:00}:{time.Seconds:00} {latency}"));
    }
}