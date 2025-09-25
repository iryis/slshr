namespace SlashrNext.Objects;

using DSharpPlus.Commands.ContextChecks.ParameterChecks;

public sealed class MaxStringLengthAttribute(int length) : ParameterCheckAttribute
{
    public int MaxLength { get; private set; } = length;
}
