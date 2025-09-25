using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SlashrNext.Utils;

public static class Helpers
{
    public static async Task EditCommandPerms(DiscordClient client, GuildDownloadCompletedEventArgs args)
    {
        var g = args.Guilds[685617157916459012];
        await g.BatchEditApplicationCommandPermissionsAsync([
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("passport").Id, [
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(803735064265490452), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(733481223992639650), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(812555422263148584), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(745458624972980234), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(808920008385757184), true)
            ]),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("change-status").Id, [
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(803735064265490452), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(733481223992639650), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(812555422263148584), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(745458624972980234), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(808920008385757184), true)
            ]),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("passport").Id, [
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(803735064265490452), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(733481223992639650), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(812555422263148584), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(745458624972980234), true),
                new DiscordApplicationCommandPermission(await g.GetRoleAsync(808920008385757184), true)
            ])
        ]);
    }

    private static DiscordApplicationCommand GetCmd(this DiscordClient client, string name)
    {
        return client.GetGuildApplicationCommandsAsync(685617157916459012).GetAwaiter().GetResult()
            .First(c => c.Name == name);
    }
}