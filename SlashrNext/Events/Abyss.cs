using DSharpPlus.Exceptions;
using SlashrNext.Utils;

namespace SlashrNext.Events;

public abstract class Abyss
{
    public static void RegisterEvents()
    {
        Slashr.client.MessageCreated += (_, args) =>
        {
            if (args.Channel.Id != "abyssChannel".GetConfigValue().GetUInt64())
                return Task.CompletedTask;
            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(2));
                if (args.Message.Pinned) return;
                try
                {
                    await args.Message.DeleteAsync();
                }
                catch (NotFoundException)
                {
                    Logger.Warn($"Could not delete abyss message [{args.Message.Id}], maybe it was already deleted?");
                }
            });
        };
    }

    public static async void TryCleanup()
    {
        try
        {
            var abyssChannel = await Slashr.client.GetChannelAsync("abyssChannel".GetConfigValue().GetUInt64());
            if (abyssChannel.GetMessagesAsync().Result.Count == 0) return;
            var abyssMsgs = await abyssChannel.GetMessagesAsync();
            await abyssChannel.DeleteMessagesAsync(abyssMsgs.Where(m => !m.Pinned));
        }
        catch (Exception e)
        {
            Logger.Warn($"Could not clean past abyss messages, there may be some lingering from bot downtime. ({e.Message})");
        }
    }
}