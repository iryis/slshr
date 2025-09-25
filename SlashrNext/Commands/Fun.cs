using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SlashrNext.Utils;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SlashrNext.Commands;

public class Fun
{

    [Command("neko")]
    [Description("Receive a random catgirl ^u^")]
    public async Task GetNeko(CommandContext ctx)
    {
        try
        {
            await ctx.DeferResponseAsync();
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync("https://nekos.life/api/v2/img/neko").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var neko = JsonSerializer.Deserialize<JsonElement>(response).GetProperty("url").GetString();
            await ctx.FollowupAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(neko ?? string.Empty)
                        .WithFooter(neko ?? string.Empty).WithDescription("nyaa~").WithColor(new DiscordColor("#B366FF")).Build())
                    .AddActionRowComponent(
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "sl_nk", "See more", false,
                            new DiscordComponentEmoji(736864625320263751)), new DiscordButtonComponent(
                            DiscordButtonStyle.Danger,
                            "sl_nk_del", "Delete", false,
                            new DiscordComponentEmoji("❌"))));
        }
        catch (Exception e)
        {
            await ctx.FollowupAsync(
                new DiscordFollowupMessageBuilder().WithContent("Unfortunately, the cat got away..."));
            Logger.Error(e);
        }
    }

    public static async Task HandleMoreButton(ComponentInteractionCreatedEventArgs ctx)
    {
        try
        {
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync("https://nekos.life/api/v2/img/neko").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var neko = JsonSerializer.Deserialize<JsonElement>(response).GetProperty("url").GetString();
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(neko ?? string.Empty)
                    .WithFooter(neko ?? string.Empty).WithDescription("nyaa~")
                    .WithColor(new DiscordColor("#B366FF")).Build()).AddActionRowComponent(
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "sl_nk", "See more", false,
                        new DiscordComponentEmoji(736864625320263751)),
                    new DiscordButtonComponent(DiscordButtonStyle.Danger, "sl_nk_del", "Delete", false,
                        new DiscordComponentEmoji("❌"))));
        }
        catch (Exception e)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Unfortunately, the cat got away..."));
            Logger.Error(e);
        }
    }
}