using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SlashrNext.Commands;

public class General : BaseCommandModule
{
    [Command("uptime")]
    public async Task BotUptime(CommandContext ctx)
    {
        var time = Slashr.uptime.Elapsed;
        await ctx.RespondAsync($"Uptime: {time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}\nPing: {Slashr.client.Ping}ms");
    }
}