using DSharpPlus;
using DSharpPlus.Entities;
using SlashrNext.Utils;

namespace SlashrNext.Events;

public abstract class Abyss
{
    public static async void TryCleanup(DiscordClient client)
    {
        try
        {
            var abyssChannel = await client.GetChannelAsync("abyssChannel".GetConfigValue().GetUInt64());
            // Collect messages to delete (non-pinned) by enumerating the async stream
            var toDelete = new List<DiscordMessage>();
            await foreach (var msg in abyssChannel.GetMessagesAsync())
            {
                if (msg.Pinned != true) toDelete.Add(msg);
            }
            if (toDelete.Count == 0) return;
            await abyssChannel.DeleteMessagesAsync(toDelete);
        }
        catch (Exception e)
        {
            Logger.Warn($"Could not clean past abyss messages, there may be some lingering from bot downtime. ({e.Message})");
        }
    }
}