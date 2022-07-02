using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TimelessEmulator.Data.Models;
using TimelessEmulator.Game;

namespace TimelessEmulator.Data;

public static class DataManager
{

    public static IReadOnlyCollection<AlternatePassiveAddition> AlternatePassiveAdditions { get; private set; }

    public static IReadOnlyCollection<AlternatePassiveSkill> AlternatePassiveSkills { get; private set; }

    public static IReadOnlyCollection<AlternateTreeVersion> AlternateTreeVersions { get; private set; }

    public static IReadOnlyCollection<PassiveSkill> PassiveSkills { get; private set; }

    public static IReadOnlyCollection<Stat> Stats { get; private set; }

    static DataManager()
    {
        AlternatePassiveAdditions = null;
        AlternatePassiveSkills = null;
        AlternateTreeVersions = null;
        PassiveSkills = null;
        Stats = null;
    }

    public static bool Initialize()
    {
        AlternatePassiveAdditions = LoadFromFile<AlternatePassiveAddition>(Settings.AlternatePassiveAdditionsFilePath);
        AlternatePassiveSkills = LoadFromFile<AlternatePassiveSkill>(Settings.AlternatePassiveSkillsFilePath);
        AlternateTreeVersions = LoadFromFile<AlternateTreeVersion>(Settings.AlternateTreeVersionsFilePath);
        PassiveSkills = LoadFromFile<PassiveSkill>(Settings.PassiveSkillsFilePath);
        Stats = LoadFromFile<Stat>(Settings.StatsFilePath);

        if ((AlternatePassiveAdditions == null) || (AlternatePassiveSkills == null) || (AlternateTreeVersions == null) || (PassiveSkills == null) || (Stats == null))
            return false;

        return true;
    }

    public static List<AlternatePassiveAddition> GetApplicableAlternatePassiveAdditions(PassiveSkill passiveSkill, TimelessJewel timelessJewel)
    {
        ArgumentNullException.ThrowIfNull(passiveSkill, nameof(passiveSkill));
        ArgumentNullException.ThrowIfNull(timelessJewel, nameof(timelessJewel));

        List<AlternatePassiveAddition> applicableAlternatePassiveAdditions = new List<AlternatePassiveAddition>();

        foreach (AlternatePassiveAddition alternatePassiveAddition in AlternatePassiveAdditions)
        {
            PassiveSkillType passiveSkillType = GetPassiveSkillType(passiveSkill);

            if ((alternatePassiveAddition.AlternateTreeVersionIndex != timelessJewel.AlternateTreeVersion.Index) ||
                !alternatePassiveAddition.ApplicablePassiveTypes.Any(q => (q == ((uint) passiveSkillType))))
            {
                continue;
            }

            applicableAlternatePassiveAdditions.Add(alternatePassiveAddition);
        }

        return applicableAlternatePassiveAdditions;
    }

    public static AlternatePassiveSkill GetAlternatePassiveSkillKeyStone(TimelessJewel timelessJewel)
    {
        ArgumentNullException.ThrowIfNull(timelessJewel, nameof(timelessJewel));

        AlternatePassiveSkill alternatePassiveSkillKeyStone = AlternatePassiveSkills.FirstOrDefault(q => (
            (q.AlternateTreeVersionIndex == timelessJewel.AlternateTreeVersion.Index) &&
            (q.ConquerorIndex == timelessJewel.TimelessJewelConqueror.Index) &&
            (q.ConquerorVersion == timelessJewel.TimelessJewelConqueror.Version)));

        if (!alternatePassiveSkillKeyStone.ApplicablePassiveTypes.Any(q => (q == ((uint) PassiveSkillType.KeyStone))))
            return null;

        return alternatePassiveSkillKeyStone;
    }

    public static List<AlternatePassiveSkill> GetApplicableAlternatePassiveSkills(PassiveSkill passiveSkill, TimelessJewel timelessJewel)
    {
        ArgumentNullException.ThrowIfNull(passiveSkill, nameof(passiveSkill));
        ArgumentNullException.ThrowIfNull(timelessJewel, nameof(timelessJewel));

        List<AlternatePassiveSkill> applicableAlternatePassiveSkills = new List<AlternatePassiveSkill>();

        foreach (AlternatePassiveSkill alternatePassiveSkill in AlternatePassiveSkills)
        {
            PassiveSkillType passiveSkillType = GetPassiveSkillType(passiveSkill);

            if ((alternatePassiveSkill.AlternateTreeVersionIndex != timelessJewel.AlternateTreeVersion.Index) ||
                !alternatePassiveSkill.ApplicablePassiveTypes.Any(q => (q == ((uint) passiveSkillType))))
            {
                continue;
            }

            applicableAlternatePassiveSkills.Add(alternatePassiveSkill);
        }

        return applicableAlternatePassiveSkills;
    }

    public static PassiveSkill GetPassiveSkillByFuzzyValue(string fuzzyValue)
    {
        ArgumentNullException.ThrowIfNull(fuzzyValue, nameof(fuzzyValue));

        if (PassiveSkills == null)
            return null;

        return PassiveSkills.FirstOrDefault(q => (
            (q.Identifier.ToLowerInvariant() == fuzzyValue.ToLowerInvariant()) ||
            (q.Name.ToLowerInvariant() == fuzzyValue.ToLowerInvariant())));
    }

    public static PassiveSkillType GetPassiveSkillType(PassiveSkill passiveSkill)
    {
        ArgumentNullException.ThrowIfNull(passiveSkill, nameof(passiveSkill));

        if (passiveSkill.IsJewelSocket)
            return PassiveSkillType.JewelSocket;

        if (passiveSkill.IsKeyStone)
            return PassiveSkillType.KeyStone;

        if (passiveSkill.IsNotable)
            return PassiveSkillType.Notable;

        if (passiveSkill.StatIndices.Count == 1)
        {
            uint bitPosition = ((passiveSkill.StatIndices.ElementAt(0) + 1) - 574);

            if ((bitPosition <= 6) && ((0x49 & (1 << ((int) bitPosition))) != 0))
                return PassiveSkillType.SmallAttribute;
        }

        return PassiveSkillType.SmallNormal;
    }

    public static bool IsPassiveSkillValidForAlteration(PassiveSkill passiveSkill)
    {
        ArgumentNullException.ThrowIfNull(passiveSkill, nameof(passiveSkill));

        PassiveSkillType passiveSkillType = GetPassiveSkillType(passiveSkill);

        return ((passiveSkillType != PassiveSkillType.None) && (passiveSkillType != PassiveSkillType.JewelSocket));
    }

    public static string GetStatIdentifierByIndex(uint index)
    {
        if (Stats == null)
            return null;

        return Stats
            .FirstOrDefault(q => (q.Index == index))
            .Identifier;
    }

    public static string GetStatTextByIndex(uint index)
    {
        if (Stats == null)
            return null;

        string statText = Stats
            .FirstOrDefault(q => (q.Index == index))
            .Text;

        if (String.IsNullOrEmpty(statText))
            statText = "<no name>";

        return statText;
    }

    private static IReadOnlyCollection<T> LoadFromFile<T>(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

        if (!File.Exists(filePath))
            return null;

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            return JsonSerializer.Deserialize<IReadOnlyCollection<T>>(fileStream);
    }

}
