using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TimelessEmulator.Data.Models;

public class AlternatePassiveSkill
{

    [JsonPropertyName("_rid")]
    public uint Index { get; init; }

    [JsonPropertyName("Id")]
    public string Identifier { get; init; }

    [JsonPropertyName("Name")]
    public string Name { get; init; }

    [JsonPropertyName("AlternateTreeVersionsKey")]
    public uint AlternateTreeVersionIndex { get; init; }

    [JsonPropertyName("StatsKeys")]
    public IReadOnlyCollection<uint> StatIndices { get; init; }

    [JsonPropertyName("Stat1Min")]
    public uint StatAMinimumValue { get; init; }

    [JsonPropertyName("Stat1Max")]
    public uint StatAMaximumValue { get; init; }

    [JsonPropertyName("Stat2Min")]
    public uint StatBMinimumValue { get; init; }

    [JsonPropertyName("Stat2Max")]
    public uint StatBMaximumValue { get; init; }

    [JsonPropertyName("Unknown10")]
    public uint StatCMinimumValue { get; init; }

    [JsonPropertyName("Unknown11")]
    public uint StatCMaximumValue { get; init; }

    [JsonPropertyName("Unknown12")]
    public uint StatDMinimumValue { get; init; }

    [JsonPropertyName("Unknown13")]
    public uint StatDMaximumValue { get; init; }

    [JsonPropertyName("PassiveType")]
    public IReadOnlyCollection<uint> ApplicablePassiveTypes { get; init; }

    [JsonPropertyName("SpawnWeight")]
    public uint SpawnWeight { get; init; }

    [JsonPropertyName("RandomMin")]
    public uint MinimumAdditions { get; init; }

    [JsonPropertyName("RandomMax")]
    public uint MaximumAdditions { get; init; }

    [JsonPropertyName("Unknown19")]
    public uint ConquerorIndex { get; init; }

    [JsonPropertyName("Unknown25")]
    public uint ConquerorVersion { get; init; }

    public AlternatePassiveSkill()
    {
        this.Index = default;
        this.Identifier = default;
        this.Name = default;
        this.AlternateTreeVersionIndex = default;
        this.StatIndices = default;
        this.StatAMinimumValue = default;
        this.StatAMaximumValue = default;
        this.StatBMinimumValue = default;
        this.StatBMaximumValue = default;
        this.StatCMinimumValue = default;
        this.StatCMaximumValue = default;
        this.StatDMinimumValue = default;
        this.StatDMaximumValue = default;
        this.ApplicablePassiveTypes = default;
        this.SpawnWeight = default;
        this.MinimumAdditions = default;
        this.MaximumAdditions = default;
        this.ConquerorIndex = default;
        this.ConquerorVersion = default;
    }

}
