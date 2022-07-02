using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TimelessEmulator.Data.Models;

public class AlternatePassiveAddition
{

    [JsonPropertyName("_rid")]
    public uint Index { get; init; }

    [JsonPropertyName("Id")]
    public string Identifier { get; init; }

    [JsonPropertyName("AlternateTreeVersionsKey")]
    public uint AlternateTreeVersionIndex { get; init; }

    [JsonPropertyName("StatsKeys")]
    public IReadOnlyCollection<uint> StatIndices { get; init; }

    [JsonPropertyName("Stat1Min")]
    public uint StatAMinimumValue { get; init; }

    [JsonPropertyName("Stat1Max")]
    public uint StatAMaximumValue { get; init; }

    [JsonPropertyName("Unknown7")]
    public uint StatBMinimumValue { get; init; }

    [JsonPropertyName("Unknown8")]
    public uint StatBMaximumValue { get; init; }

    [JsonPropertyName("PassiveType")]
    public IReadOnlyCollection<uint> ApplicablePassiveTypes { get; init; }

    [JsonPropertyName("SpawnWeight")]
    public uint SpawnWeight { get; init; }

    public AlternatePassiveAddition()
    {
        this.Index = default;
        this.Identifier = default;
        this.AlternateTreeVersionIndex = default;
        this.StatIndices = default;
        this.StatAMinimumValue = default;
        this.StatAMaximumValue = default;
        this.StatBMinimumValue = default;
        this.StatBMaximumValue = default;
        this.ApplicablePassiveTypes = default;
        this.SpawnWeight = default;
    }

}
