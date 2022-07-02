using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TimelessEmulator.Data.Models;

public class PassiveSkill
{

    [JsonPropertyName("_rid")]
    public uint Index { get; init; }

    [JsonPropertyName("Id")]
    public string Identifier { get; init; }

    [JsonPropertyName("PassiveSkillGraphId")]
    public uint GraphIdentifier { get; init; }

    [JsonPropertyName("Name")]
    public string Name { get; init; }

    [JsonPropertyName("Stats")]
    public IReadOnlyCollection<uint> StatIndices { get; init; }

    [JsonPropertyName("IsJewelSocket")]
    public bool IsJewelSocket { get; init; }

    [JsonPropertyName("IsNotable")]
    public bool IsNotable { get; init; }

    [JsonPropertyName("IsKeystone")]
    public bool IsKeyStone { get; init; }

    public PassiveSkill()
    {
        this.Index = default;
        this.Identifier = default;
        this.GraphIdentifier = default;
        this.Name = default;
        this.StatIndices = default;
        this.IsJewelSocket = default;
        this.IsNotable = default;
        this.IsKeyStone = default;
    }

}
