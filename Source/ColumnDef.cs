using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Label;

    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}

// Why defs?
// 
// Here's an example.
//
// Imagine a mod adds a stat. Ofc we will create an implied def for it and it will show up
// in the list of available columns. But, diff mode colors are wrong. Without defs the person
// would have to reference this mod in their code and then create an instance of StatColumn
// with different props and put it in an internal columns DB. It's too much work compared to
// simply writing a def.
// 
// Then there is also column sets. It would be easier to simply patch a column set to add a
// new stat column into it.
//
// But do we need custom columns to be defs too? You'd still have to reference this mod to
// make one. And then you can just hardcode everything into it.
//
// The only justification for custom columns being defs would be existence of a way to
// make a custom column without referencing this mod. And this is only possible by
// creating a StatDef.
//
// So maybe using DefModExtension on StatDefs was a better idea?
//
// But then how do you synchronize interfaces between StatDef column's DefModExtension
// and custom column? You can't use common interface because you can't define fields on
// interfaces and RW can only initialize fields.
//
// Actually, we can. Just use both, fields and properties.
public class ColumnDef : Def
{
    public float minWidth = 75f;
    public GenTable_ColumnType type = GenTable_ColumnType.Number;
    public TextAnchor textAnchor;
    public bool reverseDiffModeColors = false;
    public bool isPinned = false;

    public virtual Cell? CreateCell(ThingAlike thingAlike)
    {
        return null;
    }

    public override void ResolveReferences()
    {
        textAnchor = type == GenTable_ColumnType.Number
            ? TextAnchor.LowerRight
            : type == GenTable_ColumnType.Boolean
            ? TextAnchor.LowerCenter
            : TextAnchor.LowerLeft;
    }
}

public class StatColumnDef : ColumnDef
{
    public StatDef? useShouldShowFrom = null;
    public bool formatValue = true;
    public StatDef stat;

    public override Cell? CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);
        var shouldShowStat = useShouldShowFrom ?? stat;

        if (shouldShowStat.Worker.ShouldShowFor(statReq))
        {
            if (formatValue == false)
            {
                return new NumCell(this, stat.Worker.GetValue(statReq));
            }

            if (type == GenTable_ColumnType.Boolean)
            {
                return new BoolCell(this, stat.Worker.GetValue(statReq));
            }

            return new StatCell(this, stat, statReq);
        }

        return null;
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        stat = DefDatabase<StatDef>.GetNamed(defName);

        if (string.IsNullOrEmpty(label))
        {
            label = stat.label;
        }

        if (string.IsNullOrEmpty(description))
        {
            description = stat.description;
        }
    }
}

public class LabelColumnDef : ColumnDef
{
    public override Cell? CreateCell(ThingAlike thing)
    {
        return new StrCell(this, thing.label)
        {
            Tip = thing.def.description,
            Def = thing.def,
            Stuff = thing.stuff,
        };
    }
}

public class WeaponRangeColumnDef : ColumnDef
{
    public override Cell? CreateCell(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var range = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new NumCell(this, _range);
            }
        }

        return null;
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        label = "Range".Translate();
        description = "Stat_Thing_Weapon_Range_Desc".Translate();
    }
}
