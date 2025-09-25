using DSharpPlus.Commands;

namespace SlashrNext.Objects;

using DSharpPlus.Commands.ContextChecks.ParameterChecks;

public sealed class MaxStringLengthCheck : IParameterCheck<MaxStringLengthAttribute>
{
    public ValueTask<string?> ExecuteCheckAsync(MaxStringLengthAttribute attribute, ParameterCheckInfo info,
        CommandContext context)
    {
        if (info.Value is not string str)
        {
            return ValueTask.FromResult<string?>("The provided parameter was not a string.");
        }

        return str.Length >= attribute.MaxLength
            ? ValueTask.FromResult<string?>("The string exceeded the length limit.")
            : ValueTask.FromResult<string?>(null);
    }
}