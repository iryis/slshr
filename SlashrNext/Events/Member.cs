using DSharpPlus.Entities;
using SlashrNext.SlashCommands;
using SlashrNext.Utils;

// ReSharper disable UnusedMember.Global

namespace SlashrNext.Events;

public class Member
{
    public static void RegisterEvents()
    {
        var gate = "gateEnabled".GetConfigValue().GetBoolean();
        Slashr.client.GuildMemberAdded += (sender, args) =>
        {
            return Task.Run(async () =>
            {
                if (!gate || args.Guild.Id != 685617157916459012) return Task.CompletedTask; // got lazy
                Logger.Msg($"Guild member added: {args.Member.Username} ({args.Member.Id})");
                var user = await sender.GetUserAsync(args.Member.Id);
                var time = DateTime.Now - args.Member.CreationTimestamp.DateTime;
                if (time.Days >= 1) return Task.CompletedTask;
                if (Moderation.passported.Contains(args.Member.Id))
                {
                    Moderation.passported.Remove(args.Member.Id);
                    Moderation.SavePassported();
                    return Task.CompletedTask;
                }
                try
                {
                    await args.Member.SendMessageAsync(
                        $"Sorry, your account is too new to join {args.Guild.Name} at this time. Please try rejoining after your account is at least one day old.");
                }
                catch
                {
                    // member has dms off
                }

                await args.Member.RemoveAsync($"Account too new ({time.Hours} hours {time.Minutes} minutes)");

                await sender.GetChannelAsync("logChannel".GetConfigValue().GetUInt64()).GetAwaiter().GetResult()
                    .SendMessageAsync(
                        $"{user.Username}#{user.Discriminator} ({user.Id}) has been kicked: Account is too new");
                return Task.CompletedTask;
            });
        };

        Slashr.client.GuildMemberUpdated += (sender, args) =>
        {
            return Task.Run(async () =>
            {
                if (args.PendingBefore == true && args.PendingAfter == false)
                {
                    await sender.GetChannelAsync("logChannel".GetConfigValue().GetUInt64()).GetAwaiter().GetResult()
                        .SendMessageAsync($"{args.Member.Mention} has passed membership screening.");
                    new Thread(() =>
                    {
                        Task.Run(async () =>
                        {
                            Thread.Sleep(TimeSpan.FromMinutes(10));
                            await args.Member.GrantRoleAsync(args.Guild.GetRole(731091614453202944),
                                "Member completed rules screening");
                            await sender.GetChannelAsync("logChannel".GetConfigValue().GetUInt64()).GetAwaiter()
                                .GetResult()
                                .SendMessageAsync($"{args.Member.Mention} was given member role.");
                        });
                    }).Start();
                }
            });
        };
    }
}