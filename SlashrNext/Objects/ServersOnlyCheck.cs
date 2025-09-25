using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace SlashrNext.Objects;

public sealed class ServersOnlyCheck : IContextCheck<ServersOnlyAttribute>
{
    public ValueTask<string?> ExecuteCheckAsync(ServersOnlyAttribute attribute, CommandContext ctx)
    {
        return ctx.Guild is null
            ? ValueTask.FromResult<string?>("This command can only be used in a server.")
            : ValueTask.FromResult<string?>(null);
    }
}