using System;
using TimelessEmulator.Data.Models;

namespace TimelessEmulator.Game;

public class TimelessJewel
{

    public AlternateTreeVersion AlternateTreeVersion { get; private set; }

    public TimelessJewelConqueror TimelessJewelConqueror { get; private set; }

    public uint Seed { get; private set; }

    public TimelessJewel(AlternateTreeVersion alternateTreeVersion, TimelessJewelConqueror timelessJewelConqueror, uint seed)
    {
        ArgumentNullException.ThrowIfNull(alternateTreeVersion, nameof(alternateTreeVersion));
        ArgumentNullException.ThrowIfNull(timelessJewelConqueror, nameof(timelessJewelConqueror));

        this.AlternateTreeVersion = alternateTreeVersion;
        this.TimelessJewelConqueror = timelessJewelConqueror;
        this.Seed = seed;

        this.TransformSeed();
    }

    private void TransformSeed()
    {
        if (this.AlternateTreeVersion.Index != 5) // Using 5 as constant for the eternal (Elegant Hubris) timeless jewels is ugly.
            return;

        // The game actually stores seeds as uint16, but eternal (Elegant Hubris) timeless jewels allow seeds from 2000 to 160000.
        // This is achieved by storing a seed from 100 to 8000 and displaying this value on the item times 20.
        this.Seed /= 20;
    }

}
