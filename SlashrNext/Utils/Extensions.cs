using System.Text.Json;
using DSharpPlus.Entities;

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
            "watching" => ActivityType.Watching,
            "listening" => ActivityType.ListeningTo,
            "listeningto" => ActivityType.ListeningTo,
            "streaming" => ActivityType.Streaming,
            "competing" => ActivityType.Competing,
            "custom" => ActivityType.Custom,
            _ => ActivityType.Playing
        };
    }

    internal static DiscordActivity ParseActivity(string name, string activityType)
    {
        return new DiscordActivity(name, activityType.ParseActivityType());
    }

    internal static JsonElement GetConfigValue(this string name)
    {
        var config = Slashr.config.GetProperty(name);
        return config;
    }
    
}