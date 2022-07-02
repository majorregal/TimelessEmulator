using System;
using System.Collections.Generic;
using TimelessEmulator.Data.Models;

namespace TimelessEmulator.Game;

public class AlternatePassiveAdditionInformation
{

    public AlternatePassiveAddition AlternatePassiveAddition { get; private set; }

    public IReadOnlyDictionary<uint, uint> StatRolls { get; private set; }

    public AlternatePassiveAdditionInformation(AlternatePassiveAddition alternatePassiveAddition, IReadOnlyDictionary<uint, uint> statRolls)
    {
        ArgumentNullException.ThrowIfNull(alternatePassiveAddition, nameof(alternatePassiveAddition));

        this.AlternatePassiveAddition = alternatePassiveAddition;
        this.StatRolls = statRolls;
    }

}
