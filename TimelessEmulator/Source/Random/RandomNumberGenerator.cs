using System;
using System.Linq;
using TimelessEmulator.Data.Models;
using TimelessEmulator.Game;

namespace TimelessEmulator.Random;

public class RandomNumberGenerator
{

    private const uint INITIAL_STATE_CONSTANT_0 = 0x40336050;
    private const uint INITIAL_STATE_CONSTANT_1 = 0xCFA3723C;
    private const uint INITIAL_STATE_CONSTANT_2 = 0x3CAC5F6F;
    private const uint INITIAL_STATE_CONSTANT_3 = 0x3793FDFF;

    private uint[] state;

    public RandomNumberGenerator(PassiveSkill passiveSkill, TimelessJewel timelessJewel)
    {
        ArgumentNullException.ThrowIfNull(timelessJewel, nameof(timelessJewel));

        this.state = default;

        this.Initialize(Array.Empty<uint>()
            .Append(passiveSkill.GraphIdentifier)
            .Append(timelessJewel.Seed)
            .ToArray());
    }

    private static uint ManipulateAlpha(uint value)
    {
        return ((value ^ (value >> 27)) * 0x19660D);
    }

    private static uint ManipulateBravo(uint value)
    {
        return ((value ^ (value >> 27)) * 0x5D588B65);
    }

    public uint Generate(uint exclusiveMaximumValue)
    {
        uint maximumValue = (exclusiveMaximumValue - 1);
        uint roundState = 0;
        uint value = 0;

        do
        {
            do
            {
                value = (this.GenerateUInt() | (2 * (value << 31)));
                roundState = (0xFFFFFFFF | (2 * (roundState << 31)));
            } while (roundState < maximumValue);
        } while (((value / exclusiveMaximumValue) >= roundState) && ((roundState % exclusiveMaximumValue) != maximumValue));

        return (value % exclusiveMaximumValue);
    }

    public uint Generate(uint minimumValue, uint maximumValue)
    {
        uint a = (minimumValue + 0x80000000);
        uint b = (maximumValue + 0x80000000);

        if (minimumValue >= 0x80000000)
            a = (minimumValue + 0x80000000);

        if (maximumValue >= 0x80000000)
            b = (maximumValue + 0x80000000);

        uint roll = this.Generate(((b - a) + 1));

        return ((roll + a) + 0x80000000);
    }

    private void Initialize(uint[] seeds)
    {
        this.state = new uint[]
        {
            0,
            INITIAL_STATE_CONSTANT_0,
            INITIAL_STATE_CONSTANT_1,
            INITIAL_STATE_CONSTANT_2,
            INITIAL_STATE_CONSTANT_3
        };

        uint index = 1;

        for (int i = 0; i < seeds.Length; i++)
        {
            uint roundState = ManipulateAlpha(
                this.state[(index % 4) + 1] ^
                this.state[((index + 1) % 4) + 1] ^
                this.state[(((index + 4) - 1) % 4) + 1]);

            this.state[((index + 1) % 4) + 1] += roundState;

            roundState += (seeds[i] + index);

            this.state[(((index + 1) + 1) % 4) + 1] += roundState;
            this.state[(index % 4) + 1] = roundState;

            index = ((index + 1) % 4);
        }

        for (int i = 0; i < 5; i++)
        {
            uint roundState = ManipulateAlpha(
                this.state[(index % 4) + 1] ^
                this.state[((index + 1) % 4) + 1] ^
                this.state[(((index + 4) - 1) % 4) + 1]);

            this.state[((index + 1) % 4) + 1] += roundState;

            roundState += index;

            this.state[(((index + 1) + 1) % 4) + 1] += roundState;
            this.state[(index % 4) + 1] = roundState;

            index = ((index + 1) % 4);
        }

        for (int i = 0; i < 4; i++)
        {
            uint roundState = ManipulateBravo(
                this.state[(index % 4) + 1] +
                this.state[((index + 1) % 4) + 1] +
                this.state[(((index + 4) - 1) % 4) + 1]);

            this.state[((index + 1) % 4) + 1] ^= roundState;

            roundState -= index;

            this.state[(((index + 1) + 1) % 4) + 1] ^= roundState;
            this.state[(index % 4) + 1] = roundState;

            index = ((index + 1) % 4);
        }

        for (int i = 0; i < 8; i++)
            this.GenerateNextState();
    }

    private void GenerateNextState()
    {
        uint a = 0;
        uint b = 0;

        a = this.state[4];
        b = (((this.state[1] & 0x7FFFFFFF) ^ this.state[2]) ^ this.state[3]);

        a ^= (a << 1);
        b ^= ((b >> 1) ^ a);

        this.state[1] = this.state[2];
        this.state[2] = this.state[3];
        this.state[3] = (a ^ (b << 10));
        this.state[4] = b;

        this.state[2] ^= ((uint) ((int) (-((int) (b & 1)) & 0x8F7011EE)));
        this.state[3] ^= ((uint) ((int) (-((int) (b & 1)) & 0xFC78FF1F)));

        this.state[0]++;
    }

    private uint Temper()
    {
        uint a = this.state[4];
        uint b = (this.state[1] + (this.state[3] >> 8));

        a ^= b;

        if ((b & 1) != 0)
            a ^= 0x3793FDFF;

        return a;
    }

    private uint GenerateUInt()
    {
        this.GenerateNextState();

        return this.Temper();
    }

}
