using System;
using RimWorld;
using Verse;

namespace Stats;

public sealed class ThingAlike : IEquatable<ThingAlike>
{
    public string Label { get; }
    public ThingDef Def { get; }
    public ThingDef? Stuff { get; }
    public ThingIconWidget Icon { get; }
    private readonly int Hash;
    public ThingAlike(ThingDef def, ThingDef? stuff = null)
    {
        Def = def;
        Stuff = stuff ?? GenStuff.DefaultStuffFor(Def);
        Label = Stuff == null ? Def.LabelCap : $"{Def.LabelCap} ({Stuff.LabelCap})";
        Icon = new(Def, Stuff);
        Hash = HashCode.Combine(Def.GetHashCode(), Stuff?.GetHashCode());
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as ThingAlike);
    }
    public bool Equals(ThingAlike? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Def == other.Def && Stuff == other.Stuff;
    }
    public override int GetHashCode()
    {
        return Hash;
    }
    public static bool operator ==(ThingAlike? lhs, ThingAlike? rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            return false;
        }

        return lhs.Equals(rhs);
    }
    public static bool operator !=(ThingAlike? lhs, ThingAlike? rhs) => !(lhs == rhs);
    public static implicit operator ThingAlike(ThingDef thingDef) => new(thingDef);
}
