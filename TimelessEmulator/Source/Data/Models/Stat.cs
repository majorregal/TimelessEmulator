using System.Text.Json.Serialization;

namespace TimelessEmulator.Data.Models;

public class Stat
{

    [JsonPropertyName("_rid")]
    public uint Index { get; init; }

    [JsonPropertyName("Id")]
    public string Identifier { get; init; }

    [JsonPropertyName("Text")]
    public string Text { get; init; }

    public Stat()
    {
        this.Index = default;
        this.Identifier = default;
        this.Text = default;
    }

}
