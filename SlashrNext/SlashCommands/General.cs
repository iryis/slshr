using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SlashrNext.SlashCommands;

public class General : ApplicationCommandModule
{
    [SlashCommand("hello", "Say hi!")]
    public async Task HelloCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Hey {ctx.Member.Mention}!"));
    }

    [SlashCommand("uptime", "Check bot uptime and stats")]
    public async Task BotUptime(InteractionContext ctx)
    {
        var time = Slashr.uptime.Elapsed;
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(
                $"Uptime: {time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}\nPing: {Slashr.client.Ping}ms"));
    }
}