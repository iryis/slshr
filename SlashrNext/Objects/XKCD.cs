using System.Text.Json.Serialization;

namespace SlashrNext.Objects;

#pragma warning disable CS8618
// Non-nullable field is uninitialized. Consider declaring as nullable.
public class XKCD
{
    [JsonPropertyName("num")] public int num { get; set; }
    [JsonPropertyName("safe_title")] public string safe_title { get; set; }
    [JsonPropertyName("alt")] public string alt { get; set; }
    [JsonPropertyName("img")] public string img { get; set; }
}