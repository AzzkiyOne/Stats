using System;
using System.Linq;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

public abstract class ColumnWorker : GenTable.IColumnWorker<ThingAlike>
{
    public ColumnDef Def { get; set; }

    public abstract bool ShouldShowFor(ThingAlike thing);
    public virtual string GetCellText(ThingAlike thing) { return ""; }
    public virtual string GetCellTip(ThingAlike thing) { return ""; }
    public abstract IComparable GetCellSortValue(ThingAlike thing);
}

public class LabelColumnWorker : ColumnWorker
{
    public override bool ShouldShowFor(ThingAlike thing)
    {
        return true;
    }
    public override string GetCellText(ThingAlike thing)
    {
        return thing.Label;
    }
    public override string GetCellTip(ThingAlike thing)
    {
        return thing.Def.description;
    }
    public override IComparable GetCellSortValue(ThingAlike thing)
    {
        return thing.Label;
    }
}

public class StatColumnWorker : ColumnWorker
{
    private float GetValue(ThingAlike thing)
    {
        if (Def.Stat == null)
        {
            return float.NaN;
        }

        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        return Def.Stat.Worker.GetValue(statReq);
    }
    public override bool ShouldShowFor(ThingAlike thing)
    {
        if (Def.Stat == null)
        {
            return false;
        }

        var statReq = StatRequest.For(thing.Def, thing.Stuff);
        var shouldShowStat = Def.UseShouldShowFrom ?? Def.Stat;

        return shouldShowStat.Worker.ShouldShowFor(statReq);
    }
    public override string GetCellText(ThingAlike thing)
    {
        if (Def.Stat == null)
        {
            return "";
        }

        var statValue = GetValue(thing);

        if (Def.FormatValue)
        {
            return Def.Stat.Worker.GetStatDrawEntryLabel(
                Def.Stat,
                statValue,
                ToStringNumberSense.Absolute,
                // This is necessary (despite statReq being "optional") because some mods
                // override this signature.
                StatRequest.For(thing.Def, thing.Stuff)
            );
        }
        else
        {
            return statValue.ToString();
        }
    }
    public override IComparable GetCellSortValue(ThingAlike thing)
    {
        return GetValue(thing);
    }
}

public class WeaponRangeColumnWorker : ColumnWorker
{
    private float GetValue(ThingAlike thing)
    {
        var range = thing.Def.Verbs.First(v => v.isPrimary)?.range;

        if (range is float _range)
        {
            return _range;
        }

        return float.NaN;
    }
    public override bool ShouldShowFor(ThingAlike thing)
    {
        return thing.Def.IsRangedWeapon && thing.Def.Verbs.Count > 0;
    }
    public override string GetCellText(ThingAlike thing)
    {
        return GetValue(thing).ToString();
    }
    public override IComparable GetCellSortValue(ThingAlike thing)
    {
        return GetValue(thing);
    }
}