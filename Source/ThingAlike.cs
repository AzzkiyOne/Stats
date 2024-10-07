using System;
using System.Collections.Generic;
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
        Hash = HashCode.Combine(Def, Stuff);
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
    private static List<ThingAlike> _all;
    public static List<ThingAlike> All
    {
        get
        {
            if (_all != null)
            {
                return _all;
            }

            _all = [];

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (
                    thingDef.IsBlueprint
                    || thingDef.IsFrame
                    || thingDef.isUnfinishedThing
                    || thingDef.IsCorpse
                    ||
                        thingDef.category != ThingCategory.Pawn
                        && thingDef.category != ThingCategory.Item
                        && thingDef.category != ThingCategory.Building
                        && thingDef.category != ThingCategory.Plant
                )
                {
                    continue;
                }

                if (thingDef.MadeFromStuff)
                {
                    var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                    foreach (var stuffDef in allowedStuffs)
                    {
                        _all.Add(new ThingAlike(thingDef, stuffDef));
                    }
                }
                else
                {
                    _all.Add(new ThingAlike(thingDef));
                }
            }

            return _all;
        }
    }
}
