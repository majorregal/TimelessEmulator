using System.Text.Json.Serialization;

namespace TimelessEmulator.Data.Models;

public class AlternateTreeVersion
{

    [JsonPropertyName("_rid")]
    public uint Index { get; init; }

    [JsonPropertyName("Id")]
    public string Identifier { get; init; }

    [JsonPropertyName("Unknown2")]
    public bool AreSmallAttributePassiveSkillsReplaced { get; init; }

    [JsonPropertyName("Unknown3")]
    public bool AreSmallNormalPassiveSkillsReplaced { get; init; }

    [JsonPropertyName("Unknown6")]
    public uint MinimumAdditions { get; init; }

    [JsonPropertyName("Unknown7")]
    public uint MaximumAdditions { get; init; }

    [JsonPropertyName("Unknown10")]
    public uint NotableReplacementSpawnWeight { get; init; }

    public AlternateTreeVersion()
    {
        this.Index = default;
        this.Identifier = default;
        this.AreSmallAttributePassiveSkillsReplaced = default;
        this.AreSmallNormalPassiveSkillsReplaced = default;
        this.MinimumAdditions = default;
        this.MaximumAdditions = default;
        this.NotableReplacementSpawnWeight = default;
    }

}
