using System.Text.Json;
using DSharpPlus.Entities;
using System.Reflection;
using DSharpPlus.Commands;

namespace SlashrNext.Utils;

public static class Extensions
{
    private static DiscordActivityType ParseActivityType(this string? activity)
    {
        if (string.IsNullOrEmpty(activity)) return DiscordActivityType.Playing;
        var didParse = Enum.TryParse<DiscordActivityType>(activity, out var at);
        if (didParse) return at;
        return activity.ToLowerInvariant() switch
        {
            "playing" => DiscordActivityType.Playing,
            "streaming" => DiscordActivityType.Streaming,
            "listening" => DiscordActivityType.ListeningTo,
            "watching" => DiscordActivityType.Watching,
            "custom" => DiscordActivityType.Custom,
            "competing" => DiscordActivityType.Competing,
            _ => DiscordActivityType.Custom
        };
    }

    public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
    {
        return value.Length > maxLength
            ? string.Concat(value.AsSpan(0, maxLength), truncationSuffix)
            : value;
    }

    internal static DiscordActivity ParseActivity(string name, string DiscordActivityType)
    {
        var safeName = name.Truncate(128);
        return new DiscordActivity(safeName, DiscordActivityType.ParseActivityType());
    }

    public static DiscordEmoji? ConvertEmoji(this DiscordComponentEmoji e)
    {
        try
        {
            if (DiscordEmoji.IsValidUnicode(e.Name))
                return DiscordEmoji.FromUnicode(e.Name);
            return e.Id != 0L
                ? DiscordEmoji.FromGuildEmote(Slashr.discordClient, e.Id)
                : DiscordEmoji.FromName(Slashr.discordClient, e.Name);
        }
        catch (Exception ex)
        {
            Logger.Error($"could not convert emoji {e.Name} ({e.Id})\n {ex}");
            return null;
        }
    }

    internal static JsonElement GetConfigValue(this string name)
    {
        var config = Slashr.config.GetProperty(name);
        return config;
    }

    // Discover current command names in this assembly. These are the names that should exist as slash commands.
    internal static HashSet<string> GetDeclaredSlashCommandNames()
    {
        var asm = Assembly.GetExecutingAssembly();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in asm.GetTypes())
        {
            if (!type.IsClass) continue;

            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var cmdAttr = method.GetCustomAttribute<CommandAttribute>();
                if (cmdAttr == null) continue;

                var name = string.IsNullOrWhiteSpace(cmdAttr.Name) ? method.Name : cmdAttr.Name;
                // Discord slash command names are lowercase; keep safe by normalizing.
                names.Add(name.Trim().ToLowerInvariant());
            }
        }

        return names;
    }
}