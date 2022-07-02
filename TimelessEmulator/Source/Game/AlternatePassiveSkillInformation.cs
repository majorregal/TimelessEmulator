using System;
using System.Collections.Generic;
using TimelessEmulator.Data.Models;

namespace TimelessEmulator.Game;

public class AlternatePassiveSkillInformation
{

    public AlternatePassiveSkill AlternatePassiveSkill { get; private set; }

    public IReadOnlyDictionary<uint, uint> StatRolls { get; private set; }

    public IReadOnlyCollection<AlternatePassiveAdditionInformation> AlternatePassiveAdditionInformations { get; private set; }

    public AlternatePassiveSkillInformation(AlternatePassiveSkill alternatePassiveSkill, IReadOnlyDictionary<uint, uint> statRolls, IReadOnlyCollection<AlternatePassiveAdditionInformation> alternatePassiveAdditionInformations)
    {
        ArgumentNullException.ThrowIfNull(alternatePassiveSkill, nameof(alternatePassiveSkill));
        ArgumentNullException.ThrowIfNull(statRolls, nameof(statRolls));
        ArgumentNullException.ThrowIfNull(alternatePassiveAdditionInformations, nameof(alternatePassiveAdditionInformations));

        this.AlternatePassiveSkill = alternatePassiveSkill;
        this.StatRolls = statRolls;
        this.AlternatePassiveAdditionInformations = alternatePassiveAdditionInformations;
    }

}
