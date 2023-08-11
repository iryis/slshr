using System.Text.Json;
using DSharpPlus.Entities;
using System;
using DSharpPlus.CommandsNext.Converters;

namespace SlashrNext.Utils;

public static class Extensions
{
    private static ActivityType ParseActivityType(this string? activity)
    {
        if (string.IsNullOrEmpty(activity)) return ActivityType.Playing;
        var didParse = Enum.TryParse<ActivityType>(activity, out var at);
        if (didParse) return at;
        return activity.ToLowerInvariant() switch
        {
            "playing" => ActivityType.Playing,
            "streaming" => ActivityType.Streaming,
            "listening" => ActivityType.ListeningTo,
            "watching" => ActivityType.Watching,
            "custom" => ActivityType.Custom,
            "competing" => ActivityType.Competing,
            _ => ActivityType.Playing
        };
    }

    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? string.Concat(value.AsSpan(0, maxLength), truncationSuffix)
            : value;
    }

    internal static DiscordActivity ParseActivity(string name, string activityType)
    {
        return new DiscordActivity(name, activityType.ParseActivityType());
    }

    public static DiscordEmoji? ConvertEmoji(this DiscordComponentEmoji e)
    {
        try
        {
            if (DiscordEmoji.IsValidUnicode(e.Name))
                return DiscordEmoji.FromUnicode(e.Name);
            return e.Id != 0L
                ? DiscordEmoji.FromGuildEmote(Slashr.client, e.Id)
                : DiscordEmoji.FromName(Slashr.client, e.Name);
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
}