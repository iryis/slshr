using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SlashrNext.Utils;

public static class Helpers
{
    public static async Task EditCommandPerms(DiscordClient client, GuildDownloadCompletedEventArgs args)
    {
        var g = args.Guilds[685617157916459012];
        await g.BatchEditApplicationCommandPermissionsAsync(new[]
        {
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("passport").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("change-status").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("passport").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-addcategory").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-addrole").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-delcategory").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-delrole").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-howto").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-list").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-postcategory").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-postroles").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            }),
            new DiscordGuildApplicationCommandPermissions(client.GetCmd("rs-roleypoly").Id, new[]
            {
                new DiscordApplicationCommandPermission(g.GetRole(803735064265490452), true),
                new DiscordApplicationCommandPermission(g.GetRole(733481223992639650), true),
                new DiscordApplicationCommandPermission(g.GetRole(812555422263148584), true),
                new DiscordApplicationCommandPermission(g.GetRole(745458624972980234), true),
                new DiscordApplicationCommandPermission(g.GetRole(808920008385757184), true)
            })
        });
    }

    private static DiscordApplicationCommand GetCmd(this DiscordClient client, string name)
    {
        return client.GetGuildApplicationCommandsAsync(685617157916459012).GetAwaiter().GetResult()
            .First(c => c.Name == name);
    }
}