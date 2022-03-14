using DSharpPlus;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext.SlashCommands;

// this whole thing is so bad guh
public class ReactRoles : ApplicationCommandModule
{
    // name, category
    public static List<KeyValuePair<string, RoleCategory>> Categories = new();

    [SlashCommand("rs-postroles", "Post the role selection message for the roles channel", false)]
    [SlashRequireMod]
    public async Task PostRoles(InteractionContext ctx,
        [Option("channel", "Channel to send the messages to")]
        DiscordChannel channel)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Posting roles...").AsEphemeral());
        foreach (var (name, cat) in Categories)
        {
            var rolesList = new List<string>();
            cat.roles.ForEach(r =>
                rolesList.Add($"{r.Value.emoji.ConvertEmoji()} - {r.Value.baseRole.Mention}: {r.Value.description}"));

            var btnList = cat.roles.Select(r =>
                new DiscordButtonComponent(ButtonStyle.Primary, $"sl_rr_{r.Key.Id}", r.Key.Name.Truncate(78), false,
                    r.Value.emoji)).ToList();

            var embed = new DiscordEmbedBuilder().WithColor(new DiscordColor("#B7CEF7")).WithTitle(name)
                .WithDescription($"{cat.description}".Truncate(4096))
                .AddField("Roles", string.Join("\n", rolesList).Truncate(1024), true);

            var builder = new DiscordMessageBuilder().WithEmbed(embed.Build());
            var buttons = btnList.Chunk(5);
            foreach (var btn in buttons)
            {
                builder.AddComponents(btn.ToList());
            }

            try
            {
                await channel.SendMessageAsync(builder);
            }
            catch (BadRequestException ex)
            {
                Logger.Error(ex.Errors);
            }
        }

        await ctx.EditResponseAsync(
            new DiscordWebhookBuilder().WithContent(
                $"Done! Run /rs-roleypoly in {channel.Mention} to post the roleypoly message."));
    }


    [SlashCommand("rs-postcategory", "Post one role select message, useful for event roles. Use postroles for all",
        false)]
    [SlashRequireMod]
    public async Task PostCategory(InteractionContext ctx, [Option("category", "The category to post")] string category,
        [Option("channel", "Channel to send the messages to")]
        DiscordChannel channel)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Posting {category}...").AsEphemeral());

        var cat = Categories.First(c => c.Key == category);
        var rolesList = new List<string>();
        cat.Value.roles.ForEach(r =>
            rolesList.Add($"{r.Value.emoji.ConvertEmoji()} - {r.Value.baseRole.Mention}: {r.Value.description}"));

        var btnList = cat.Value.roles.Select(r =>
            new DiscordButtonComponent(ButtonStyle.Primary, $"sl_rr_{r.Key.Id}", r.Key.Name.Truncate(78), false,
                r.Value.emoji)).ToList();

        var embed = new DiscordEmbedBuilder().WithColor(new DiscordColor("#B7CEF7")).WithTitle(cat.Value.name)
            .WithDescription($"{cat.Value.description}".Truncate(4096))
            .AddField("Roles", string.Join("\n", rolesList).Truncate(1024), true);

        var builder = new DiscordMessageBuilder().WithEmbed(embed.Build());
        var buttons = btnList.Chunk(5);
        foreach (var btn in buttons)
        {
            builder.AddComponents(btn.ToList());
        }

        try
        {
            await channel.SendMessageAsync(builder);
        }
        catch (BadRequestException ex)
        {
            Logger.Error(ex.Errors);
        }
    }

    [SlashCommand("rs-addrole", "Add a role to the role selector. No more than 25 roles in a category.", false)]
    [SlashRequireMod]
    public async Task AddRole(InteractionContext ctx, [Option("category", "Role category to add to.")] string category,
        [Option("role", "the role to add")] DiscordRole role,
        [Option("description", "Role description")]
        string desc = "",
        [Option("emoji_id",
            "Emoji used in buttons, if you dont know how just use a default discord emoji")]
        string emoji = "")
    {
        await ctx.DeferAsync();
        var cat = GetRoleCategory(category);
        var roleEmoji = new DiscordComponentEmoji("⭐");
        try
        {
            if (DiscordEmoji.IsValidUnicode(emoji))
            {
                roleEmoji = new DiscordComponentEmoji(emoji);
            }
            else
            {
                if (ulong.TryParse(emoji, out var emojiId))
                {
                    roleEmoji = new DiscordComponentEmoji(emojiId);
                }
            }
        }
        catch
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Role has invalid emoji")
                .AsEphemeral(true));
            return;
        }

        try
        {
            var added = cat.AddRole(role, desc, roleEmoji);
            if (!added)
            {
                await ctx.FollowUpAsync(
                    new DiscordFollowupMessageBuilder().WithContent(
                        $"{role.Name} already exists in category {cat.name}"));
            }

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Role added").AsEphemeral(true));
        }
        catch
        {
            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder().WithContent($"No category with name {cat.name} exists")
                    .AsEphemeral(true));
        }
    }

    [SlashCommand("rs-addcategory",
        "Add a category (new message) for roles. These will likely be empty after a bot restart.", false)]
    [SlashRequireMod]
    public async Task AddRoleCategory(InteractionContext ctx,
        [Option("name", "Role category name. This will show in the embed title")]
        string name,
        [Option("desc", "Role category description. This will show in embed body")]
        string desc)
    {
        await ctx.DeferAsync();
        if (Categories.Any(c => string.Equals(c.Key, name, StringComparison.CurrentCultureIgnoreCase)))
        {
            // ily copilot 
            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder().WithContent($"A category names {name} already exists!")
                    .AsEphemeral(true));
            return;
        }

        Categories.Add(new KeyValuePair<string, RoleCategory>(name, new RoleCategory(name, desc)));
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"Category {name} added!")
            .AsEphemeral(true));
    }

    [SlashCommand("rs-delcategory", "Delete a role category", false)]
    [SlashRequireMod]
    public async Task RemoveRoleCategory(InteractionContext ctx,
        [Option("name", "Role category name.")]
        string name)
    {
        await ctx.DeferAsync(true);
        if (!Categories.Any(c => string.Equals(c.Key, name, StringComparison.CurrentCultureIgnoreCase)))
        {
            // ily copilot 
            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder()
                    .WithContent($"Couldnt find a category with the name {name}! It probably wasn't added.")
                    .AsEphemeral(true));
            return;
        }

        Categories.Remove(Categories.First(c => string.Equals(c.Key, name, StringComparison.CurrentCultureIgnoreCase)));
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"Category {name} removed!"));
    }

    [SlashCommand("rs-delrole", "Remove a role", false)]
    [SlashRequireMod]
    public async Task RemoveRole(InteractionContext ctx,
        [Option("category", "Role category to remove from")]
        string category,
        [Option("role", "the role to remove")] DiscordRole role)
    {
        await ctx.DeferAsync();
        var cat = GetRoleCategory(category);

        cat.RemoveRole(role);
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Role removed").AsEphemeral(true));
    }

    [SlashCommand("rs-list", "List added role categories", false)]
    [SlashRequireMod]
    public async Task ListCategories(InteractionContext ctx)
    {
        await ctx.DeferAsync(true);
        try
        {
            var cats = new List<string>();
            Categories.ForEach(c => cats.Add($"**{c.Key}**: {c.Value.description} ({c.Value.roles.Count} roles)"));
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(string.Join("\n", cats)));
        }
        catch
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Oops, couldnt get categories"));
        }
    }

    [SlashCommand("rs-howto", "Tutorial on how to create role selects", false)]
    [SlashRequireMod]
    public async Task Tutorial(InteractionContext ctx)
    {
        var msg = @"*Roles how-to:*
All commands starting with `/rs-` are for role setup. These are all slash commands btw

**Categories**
A role category is one message with an embed describing the category, and buttons to replace the reaction based model current bots use.
Add a category by using `rs-addcategory` and provide the necessary options. List categories with `rs-list`
Delete using.. yeah you get the point

⚠️  Categories are temporary and will be lost after a bot restart. *This does not affect already posted role messages*
**Roles**
You can add a role to a category by using `rs-addrole`, giving the category's name plus the role to add.

Repeat this for every category and role you want to add. 

**Posting**
Posting is simple. Use `rs-postcategory` to post only one role-category message. Provide the category's name and the channel to send it to.

To post all of the categories you've created thus far, use `rs-postroles` and give a channel to send it all to. You can use this again if you mess up, as long as the bot hasn't restarted.";
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent(msg));
    }

    [SlashCommand("rs-roleypoly", "Post roleypoly message with buttons", false)]
    [SlashRequireMod]
    public async Task PostRoleypoly(InteractionContext ctx)
    {
        try
        {
            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                .WithContent("Click here to assign roles through Roleypoly!").AddComponents(
                    new DiscordLinkButtonComponent($"https://roleypoly.com/s/{ctx.Guild.Id}",
                        "Roleypoly", false, new DiscordComponentEmoji("🍉"))));
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Done!").AsEphemeral());
        }
        catch
        {
            await ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().WithContent("Oops, couldnt post roleypoly message")
                    .AsEphemeral());
        }
    }

    private static RoleCategory GetRoleCategory(string catName)
    {
        return Categories.FirstOrDefault(c => string.Equals(c.Key, catName, StringComparison.CurrentCultureIgnoreCase))
            .Value;
    }
}