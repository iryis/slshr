using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace SlashrNext.Events;

public class RoleButton
{
    public static void RegisterEvents()
    {
        Slashr.client.ComponentInteractionCreated += (_, args) =>
        {
            return Task.Run(async () =>
            {
                if (!args.Id.StartsWith("sl_rr_")) return;
                var member = await args.Guild.GetMemberAsync(args.User.Id);
                var role = args.Guild.GetRole(ulong.Parse(args.Id.Replace("sl_rr_", "")));
                try
                {
                    if (!member.Roles.Contains(role))
                    {
                        await member.GrantRoleAsync(role, "React role");
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent($"You now have {role.Mention}!")
                                .AsEphemeral());
                    }
                    else
                    {
                        await member.RevokeRoleAsync(role);
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent($"You no longer have {role.Mention}!")
                                .AsEphemeral());
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(UnauthorizedException))
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"I don't have permissions to give you {role.Mention}!").AsEphemeral());   
                    }
                    else
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"An error occured.\n```{e.Message}```").AsEphemeral());
                    }
                }
            });
        };
    }
}