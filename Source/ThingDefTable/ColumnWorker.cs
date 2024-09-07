using System;
using System.Linq;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

public abstract class ColumnWorker : GenTable.IColumnWorker<ThingAlike>
{
    public ColumnDef Column { get; set; }

    public abstract bool ShouldShowFor(ThingAlike thing);
    public virtual string GetCellText(ThingAlike thing) { return ""; }
    public virtual string GetCellTip(ThingAlike thing) { return ""; }
    public abstract IComparable GetCellSortValue(ThingAlike thing);
    public virtual DefReference? GetDefRef(ThingAlike thing)
    {
        return null;
    }
}

public class ColumnWorker_Label : ColumnWorker
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
    public override DefReference? GetDefRef(ThingAlike thing)
    {
        return new DefReference(thing.Def, thing.Stuff);
    }
}

public class StatColumnWorker : ColumnWorker
{
    protected virtual StatDef Stat => Column.Stat;
    private float GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        return Stat.Worker.GetValue(statReq);
    }
    public override bool ShouldShowFor(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        return Stat.Worker.ShouldShowFor(statReq);
    }
    public override string GetCellText(ThingAlike thing)
    {
        var statValue = GetValue(thing);

        if (Column.FormatValue)
        {
            return Stat.Worker.GetStatDrawEntryLabel(
                Stat,
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

public class ColumnWorker_WeaponRange : ColumnWorker
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