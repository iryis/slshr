using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SlashrNext.Commands;

public class Help : BaseCommandModule
{
    [Command("help")]
    public async Task HelpCommand(CommandContext ctx)
    {
        await ctx.RespondAsync("Hello! Commands are available as slash commands. Type / to see them :)");
    }
}