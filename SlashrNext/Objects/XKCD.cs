using System.Text.Json.Serialization;

namespace SlashrNext.Objects;

#pragma warning disable CS8618
// Non-nullable field is uninitialized. Consider declaring as nullable.
public class XKCD
{
    [JsonPropertyName("month")] public string month { get; set; }
    [JsonPropertyName("num")] public int num { get; set; }
    [JsonPropertyName("link")] public string link { get; set; }
    [JsonPropertyName("year")] public string year { get; set; }
    [JsonPropertyName("news")] public string news { get; set; }
    [JsonPropertyName("safe_title")] public string safe_title { get; set; }
    [JsonPropertyName("transcript")] public string transcript { get; set; }
    [JsonPropertyName("alt")] public string alt { get; set; }
    [JsonPropertyName("img")] public string img { get; set; }
    [JsonPropertyName("title")] public string title { get; set; }
    [JsonPropertyName("day")] public string day { get; set; }
}