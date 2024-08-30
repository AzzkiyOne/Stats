using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef ExCellColumn;

    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}

public class ColumnDef : Def
{
    public float minWidth = 75f;
    public GenTable_ColumnType type = GenTable_ColumnType.Number;
    public TextAnchor textAnchor;
    public bool reverseDiffModeColors = false;

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
