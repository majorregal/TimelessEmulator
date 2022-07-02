using System;
using System.Collections.Generic;
using System.Linq;
using TimelessEmulator.Data;
using TimelessEmulator.Data.Models;
using TimelessEmulator.Random;

namespace TimelessEmulator.Game;

public class AlternateTreeManager
{

    public PassiveSkill PassiveSkill { get; private set; }

    public TimelessJewel TimelessJewel { get; private set; }

    public AlternateTreeManager(PassiveSkill passiveSkill, TimelessJewel timelessJewel)
    {
        ArgumentNullException.ThrowIfNull(passiveSkill, nameof(passiveSkill));
        ArgumentNullException.ThrowIfNull(timelessJewel, nameof(timelessJewel));

        this.PassiveSkill = passiveSkill;
        this.TimelessJewel = timelessJewel;
    }

    public bool IsPassiveSkillReplaced()
    {
        if (this.PassiveSkill.IsKeyStone)
            return true;

        if (this.PassiveSkill.IsNotable)
        {
            if (this.TimelessJewel.AlternateTreeVersion.NotableReplacementSpawnWeight >= 100)
                return true;

            RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator(this.PassiveSkill, this.TimelessJewel);

            return (randomNumberGenerator.Generate(0, 100) < this.TimelessJewel.AlternateTreeVersion.NotableReplacementSpawnWeight);
        }

        if (this.PassiveSkill.StatIndices.Count == 1)
        {
            uint bitPosition = ((this.PassiveSkill.StatIndices.ElementAt(0) + 1) - 574);

            if ((bitPosition <= 6) && ((0x49 & (1 << ((int) bitPosition))) != 0))
                return this.TimelessJewel.AlternateTreeVersion.AreSmallAttributePassiveSkillsReplaced;
        }

        return this.TimelessJewel.AlternateTreeVersion.AreSmallNormalPassiveSkillsReplaced;
    }

    public AlternatePassiveSkillInformation ReplacePassiveSkill()
    {
        if (this.PassiveSkill.IsKeyStone)
        {
            AlternatePassiveSkill alternatePassiveSkillKeyStone = DataManager.GetAlternatePassiveSkillKeyStone(this.TimelessJewel);
            Dictionary<uint, uint> alternatePassiveSkillKeyStoneStatRolls = new Dictionary<uint, uint>()
            {
                { 0, alternatePassiveSkillKeyStone.StatAMinimumValue }
            };

            return new AlternatePassiveSkillInformation(alternatePassiveSkillKeyStone, alternatePassiveSkillKeyStoneStatRolls, new List<AlternatePassiveAdditionInformation>());
        }

        List<AlternatePassiveSkill> applicableAlternatePassiveSkills = DataManager.GetApplicableAlternatePassiveSkills(this.PassiveSkill, this.TimelessJewel);

        AlternatePassiveSkill rolledAlternatePassiveSkill = null;
        RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator(this.PassiveSkill, this.TimelessJewel);
        uint currentSpawnWeight = 0;

        if (DataManager.GetPassiveSkillType(this.PassiveSkill) == PassiveSkillType.Notable)
            randomNumberGenerator.Generate(0, 100);

        foreach (AlternatePassiveSkill applicableAlternatePassiveSkill in applicableAlternatePassiveSkills)
        {
            currentSpawnWeight += applicableAlternatePassiveSkill.SpawnWeight;

            uint roll = randomNumberGenerator.Generate(currentSpawnWeight);

            if (roll < applicableAlternatePassiveSkill.SpawnWeight)
                rolledAlternatePassiveSkill = applicableAlternatePassiveSkill;
        }

        Dictionary<uint, (uint minimumRoll, uint maximumRoll)> alternatePassiveSkillStatRollRanges = new Dictionary<uint, (uint minimumRoll, uint maximumRoll)>()
        {
            { 0, (rolledAlternatePassiveSkill.StatAMinimumValue, rolledAlternatePassiveSkill.StatAMaximumValue) },
            { 1, (rolledAlternatePassiveSkill.StatBMinimumValue, rolledAlternatePassiveSkill.StatBMaximumValue) },
            { 2, (rolledAlternatePassiveSkill.StatCMinimumValue, rolledAlternatePassiveSkill.StatCMaximumValue) },
            { 3, (rolledAlternatePassiveSkill.StatDMinimumValue, rolledAlternatePassiveSkill.StatDMaximumValue) }
        };

        Dictionary<uint, uint> alternatePassiveSkillStatRolls = new Dictionary<uint, uint>();

        // Capping the maximum iterations at 4 for now.
        for (uint i = 0; i < Math.Min(rolledAlternatePassiveSkill.StatIndices.Count, 4); i++)
        {
            uint alternatePassiveSkillStatRoll = alternatePassiveSkillStatRollRanges[i].minimumRoll;

            if (alternatePassiveSkillStatRollRanges[i].maximumRoll > alternatePassiveSkillStatRollRanges[i].minimumRoll)
                alternatePassiveSkillStatRoll = randomNumberGenerator.Generate(alternatePassiveSkillStatRollRanges[i].minimumRoll, alternatePassiveSkillStatRollRanges[i].maximumRoll);

            alternatePassiveSkillStatRolls.Add(i, alternatePassiveSkillStatRoll);
        }

        if ((rolledAlternatePassiveSkill.MinimumAdditions == 0) && (rolledAlternatePassiveSkill.MaximumAdditions == 0))
            return new AlternatePassiveSkillInformation(rolledAlternatePassiveSkill, alternatePassiveSkillStatRolls, new List<AlternatePassiveAdditionInformation>());

        uint minimumAdditions = (this.TimelessJewel.AlternateTreeVersion.MinimumAdditions + rolledAlternatePassiveSkill.MinimumAdditions);
        uint maximumAdditions = (this.TimelessJewel.AlternateTreeVersion.MaximumAdditions + rolledAlternatePassiveSkill.MaximumAdditions);

        uint additionCountRoll = minimumAdditions;

        if (maximumAdditions > minimumAdditions)
            additionCountRoll = randomNumberGenerator.Generate(minimumAdditions, maximumAdditions);

        List<AlternatePassiveAdditionInformation> alternatePassiveAdditionInformations = new List<AlternatePassiveAdditionInformation>();

        for (int i = 0; i < additionCountRoll; i++)
        {
            AlternatePassiveAddition rolledAlternatePassiveAddition = null;

            while (rolledAlternatePassiveAddition == null)
                rolledAlternatePassiveAddition = this.RollAlternatePassiveAddition(randomNumberGenerator);

            Dictionary<uint, (uint minimumRoll, uint maximumRoll)> alternatePassiveAdditionStatRollRanges = new Dictionary<uint, (uint minimumRoll, uint maximumRoll)>()
            {
                { 0, (rolledAlternatePassiveAddition.StatAMinimumValue, rolledAlternatePassiveAddition.StatAMaximumValue) },
                { 1, (rolledAlternatePassiveAddition.StatBMinimumValue, rolledAlternatePassiveAddition.StatBMaximumValue) }
            };

            Dictionary<uint, uint> alternatePassiveAdditionStatRolls = new Dictionary<uint, uint>();

            // Capping the maximum iterations at 2 for now.
            for (uint j = 0; j < Math.Min(rolledAlternatePassiveAddition.StatIndices.Count, 2); j++)
            {
                uint alternatePassiveAdditionStatRoll = alternatePassiveAdditionStatRollRanges[j].minimumRoll;

                if (alternatePassiveAdditionStatRollRanges[j].maximumRoll > alternatePassiveAdditionStatRollRanges[j].minimumRoll)
                    alternatePassiveAdditionStatRoll = randomNumberGenerator.Generate(alternatePassiveAdditionStatRollRanges[j].minimumRoll, alternatePassiveAdditionStatRollRanges[j].maximumRoll);

                alternatePassiveAdditionStatRolls.Add(j, alternatePassiveAdditionStatRoll);
            }

            alternatePassiveAdditionInformations.Add(new AlternatePassiveAdditionInformation(rolledAlternatePassiveAddition, alternatePassiveAdditionStatRolls));
        }

        return new AlternatePassiveSkillInformation(rolledAlternatePassiveSkill, alternatePassiveSkillStatRolls, alternatePassiveAdditionInformations);
    }

    public IReadOnlyCollection<AlternatePassiveAdditionInformation> AugmentPassiveSkill()
    {
        RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator(this.PassiveSkill, this.TimelessJewel);

        if (DataManager.GetPassiveSkillType(this.PassiveSkill) == PassiveSkillType.Notable)
            randomNumberGenerator.Generate(0, 100);

        uint minimumAdditions = this.TimelessJewel.AlternateTreeVersion.MinimumAdditions;
        uint maximumAdditions = this.TimelessJewel.AlternateTreeVersion.MaximumAdditions;

        uint additionCountRoll = minimumAdditions;

        if (maximumAdditions > minimumAdditions)
            additionCountRoll = randomNumberGenerator.Generate(minimumAdditions, maximumAdditions);

        List<AlternatePassiveAdditionInformation> alternatePassiveAdditionInformations = new List<AlternatePassiveAdditionInformation>();

        for (int i = 0; i < additionCountRoll; i++)
        {
            AlternatePassiveAddition rolledAlternatePassiveAddition = null;

            while (rolledAlternatePassiveAddition == null)
                rolledAlternatePassiveAddition = this.RollAlternatePassiveAddition(randomNumberGenerator);

            Dictionary<uint, (uint minimumRoll, uint maximumRoll)> alternatePassiveAdditionStatRollRanges = new Dictionary<uint, (uint minimumRoll, uint maximumRoll)>()
            {
                { 0, (rolledAlternatePassiveAddition.StatAMinimumValue, rolledAlternatePassiveAddition.StatAMaximumValue) },
                { 1, (rolledAlternatePassiveAddition.StatBMinimumValue, rolledAlternatePassiveAddition.StatBMaximumValue) }
            };

            Dictionary<uint, uint> alternatePassiveAdditionStatRolls = new Dictionary<uint, uint>();

            // Capping the maximum iterations at 2 for now.
            for (uint j = 0; j < Math.Min(rolledAlternatePassiveAddition.StatIndices.Count, 2); j++)
            {
                uint alternatePassiveAdditionStatRoll = alternatePassiveAdditionStatRollRanges[j].minimumRoll;

                if (alternatePassiveAdditionStatRollRanges[j].maximumRoll > alternatePassiveAdditionStatRollRanges[j].minimumRoll)
                    alternatePassiveAdditionStatRoll = randomNumberGenerator.Generate(alternatePassiveAdditionStatRollRanges[j].minimumRoll, alternatePassiveAdditionStatRollRanges[j].maximumRoll);

                alternatePassiveAdditionStatRolls.Add(j, alternatePassiveAdditionStatRoll);
            }

            alternatePassiveAdditionInformations.Add(new AlternatePassiveAdditionInformation(rolledAlternatePassiveAddition, alternatePassiveAdditionStatRolls));
        }

        return alternatePassiveAdditionInformations;
    }

    private AlternatePassiveAddition RollAlternatePassiveAddition(RandomNumberGenerator randomNumberGenerator)
    {
        ArgumentNullException.ThrowIfNull(randomNumberGenerator, nameof(randomNumberGenerator));

        List<AlternatePassiveAddition> applicableAlternatePassiveAdditions = DataManager.GetApplicableAlternatePassiveAdditions(this.PassiveSkill, this.TimelessJewel);

        uint totalSpawnWeight = ((uint) applicableAlternatePassiveAdditions
               .Sum(q => q.SpawnWeight));

        uint additionRoll = randomNumberGenerator.Generate(totalSpawnWeight);

        foreach (AlternatePassiveAddition applicableAlternatePassiveAddition in applicableAlternatePassiveAdditions)
        {
            if (applicableAlternatePassiveAddition.SpawnWeight > additionRoll)
                return applicableAlternatePassiveAddition;

            additionRoll -= applicableAlternatePassiveAddition.SpawnWeight;
        }

        return null;
    }

}
