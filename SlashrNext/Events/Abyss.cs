using SlashrNext.SlashCommands;
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
                await args.Message.DeleteAsync();
            });
        };
    }

    public static async void TryCleanup()
    {
        try
        {
            var abyssChannel = await Slashr.client.GetChannelAsync("abyssChannel".GetConfigValue().GetUInt64());
            if (abyssChannel.GetMessagesAsync().Result.Count == 0) return;
            await abyssChannel.DeleteMessagesAsync(await abyssChannel.GetMessagesAsync());
        }
        catch
        {
            Logger.Warn("Could not clean past abyss messages, there may be some lingering from bot downtime.");
        }
    }
}