using System.Net;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using SlashrNext.Objects;
using SlashrNext.Utils;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SlashrNext.SlashCommands;

public class Fun : ApplicationCommandModule
{
    [SlashCommand("xkcd", "Get the latest xkcd comic")]
    public async Task GetComic(InteractionContext ctx, [Option("num", "The xkcd comic #")] string cNum = "")
    {
        try
        {
            await ctx.DeferAsync();
            var parsed = int.TryParse(cNum, out var num);
            if (num < 1) parsed = false;
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync($"https://xkcd.com/{(parsed ? $"{num}/" : "")}info.0.json").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
            if (await JsonSerializer.DeserializeAsync(response, typeof(XKCD)) is not XKCD comic)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Failed to fetch xkcd comic."));
                return;
            }

            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle(comic.safe_title).WithUrl($"https://xkcd.com/{comic.num}")
                    .WithImageUrl(comic.img).WithFooter(comic.alt).WithColor(new DiscordColor("#DEDDFF"))
                    .Build()));
        }
        catch
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Failed to fetch xkcd comic."));
        }
    }

    [SlashCommand("neko", "Receive a random catgirl ^u^")]
    public async Task GetNeko(InteractionContext ctx)
    {
        try
        {
            await ctx.DeferAsync();
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync("https://nekos.life/api/v2/img/neko").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var neko = JsonSerializer.Deserialize<JsonElement>(response).GetProperty("url").GetString();
            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(neko)
                    .WithFooter("nya~").WithColor(new DiscordColor("#B366FF")).Build()).AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, "sl_nk", "See more", false,
                        new DiscordComponentEmoji(736864625320263751)), new DiscordButtonComponent(ButtonStyle.Danger,
                        "sl_nk_del", "Delete", false,
                        new DiscordComponentEmoji("❌"))));
        }
        catch (Exception e)
        {
            await ctx.FollowUpAsync(
                new DiscordFollowupMessageBuilder().WithContent("Unfortunately, the cat got away..."));
            Logger.Error(e);
        }
    }

    public static async Task HandleMoreButton(ComponentInteractionCreateEventArgs ctx)
    {
        try
        {
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync("https://nekos.life/api/v2/img/neko").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var neko = JsonSerializer.Deserialize<JsonElement>(response).GetProperty("url").GetString();
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(neko)
                    .WithFooter("powered by nekos.life API").WithDescription("nyaa~")
                    .WithColor(new DiscordColor("#B366FF")).Build()).AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, "sl_nk", "See more", false,
                        new DiscordComponentEmoji(736864625320263751)),
                    new DiscordButtonComponent(ButtonStyle.Danger, "sl_nk_del", "Delete", false,
                        new DiscordComponentEmoji("❌"))));
        }
        catch (Exception e)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Unfortunately, the cat got away..."));
            Logger.Error(e);
        }
    }
}