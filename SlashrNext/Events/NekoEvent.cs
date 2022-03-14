using SlashrNext.SlashCommands;

namespace SlashrNext.Events;

public class NekoEvent
{
    public static void RegisterEvents()
    {
        Slashr.client.ComponentInteractionCreated += (_, args) =>
        {
            return Task.Run(async () =>
            {
                switch (args.Id)
                {
                    case "sl_nk":
                        await Fun.HandleMoreButton(args);
                        break;
                    case "sl_nk_del":
                        await args.Message.DeleteAsync();
                        break;
                }
            });
        };
    }
}