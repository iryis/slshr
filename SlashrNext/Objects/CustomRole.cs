using DSharpPlus.Entities;

namespace SlashrNext.Objects;

public class CustomRole
{
    public DiscordRole baseRole;

    public string description;

    public DiscordComponentEmoji emoji;

    public CustomRole(DiscordRole @base, string desc, DiscordComponentEmoji emoji)
    {
        baseRole = @base;
        description = desc;
        this.emoji = emoji;
    }
}