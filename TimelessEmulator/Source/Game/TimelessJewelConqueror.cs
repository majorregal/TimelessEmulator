namespace TimelessEmulator.Game;

public class TimelessJewelConqueror
{

    public uint Index { get; private set; }

    public uint Version { get; private set; }

    public TimelessJewelConqueror(uint index, uint version)
    {
        this.Index = index;
        this.Version = version;
    }

}
