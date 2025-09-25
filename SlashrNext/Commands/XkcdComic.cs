using System.ComponentModel;
using System.Text.Json;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using SlashrNext.Objects;
using SlashrNext.Utils;

namespace SlashrNext.Commands;

public class XkcdComic
{
    [Command("xkcd")]
    [Description("Fetch a comic from xkcd.com")]
    public async Task Xkcd(SlashCommandContext ctx, string cNum = "")
    {
        try
        {
            await ctx.DeferResponseAsync();
            var parsed = int.TryParse(cNum, out var num);
            if (num < 1) parsed = false;
            using var client = new HttpClient();
            var response =
                await client
                    .GetAsync($"https://xkcd.com/{(parsed ? $"{num}/" : "")}info.0.json").GetAwaiter().GetResult()
                    .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
            if (await JsonSerializer.DeserializeAsync(response, typeof(XKCD)) is not XKCD comic)
            {
                await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("Failed to fetch xkcd comic. D"));
                return;
            }

            await ctx.FollowupAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle(comic.safe_title).WithUrl($"https://xkcd.com/{comic.num}")
                    .WithImageUrl(comic.img).WithFooter(comic.alt).WithColor(new DiscordColor("#DEDDFF"))
                    .Build()));
        }
        catch (Exception err)
        {
            Logger.Error($"Failed to fetch xkcd comic {cNum}: {err}");
            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder().WithContent("Failed to fetch xkcd comic. "));
        }
    }
}