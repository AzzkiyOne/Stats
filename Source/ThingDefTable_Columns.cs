using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Stats;

static class ThingDefTable_Columns
{
    static public readonly Dictionary<string, ThingDefTable_Column> list = [];
    static ThingDefTable_Columns()
    {
        list.Add("Label", new LabelColumn());
        list.Add("WeaponRange", new ThingDefTable_WeaponRangeColumn());
        list.Add("ReloadTime", new ThingDefTable_CEReloadTimeColumn());
        list.Add("MagazineCapacity", new ThingDefTable_CEMagCapColumn());
        list.Add("OneHandedness", new ThingDefTable_CEOneHandednessColumn());

        foreach (var statDef in DefDatabase<StatDef>.AllDefs)
        {
            var column = new ThingDefTable_StatDefColumn(statDef);

            list.TryAdd(statDef.defName, column);
        }
    }
}

public abstract class ThingDefTable_Column : GenTable_Column
{
    public ThingDefTable_Column() { }

    public abstract IGenTable_Cell CreateCell(ThingAlike thingAlike);
}

public class ThingDefTable_StatDefColumn : ThingDefTable_Column
{
    private readonly StatDef _stat;
    protected StatDef Stat
    {
        get => _stat;
        private init
        {
            _stat = value;
            Description = value.description;

            var modExt = value.GetModExtension<StatDefExtension>();
            var label = value.LabelCap;

            if (modExt is not null)
            {
                DiffMult = modExt.diffMult;
                MinWidth = modExt.minWidth;
                Type = modExt.type;

                if (modExt.label != "")
                {
                    label = modExt.label.CapitalizeFirst();
                }
            }

            Label = label;
        }
    }

    public ThingDefTable_StatDefColumn(StatDef stat)
    {
        Stat = stat;
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (Stat.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_StatCell(Stat, statReq);
        }

        //return new GenTable_StatCell(
        //    statDef,
        //    float.NaN
        //);
        return new GenTable_NumCell();
    }
}

public class LabelColumn : ThingDefTable_Column
{
    public LabelColumn()
    {
        Label = "Name";
        MinWidth = 250f;
        Type = GenTable_ColumnType.String;
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        return new GenTable_StrCell(thing.label)
        {
            Tip = thing.def.description,
            Def = thing.def,
            Stuff = thing.stuff,
        };
    }
}

public class ThingDefTable_WeaponRangeColumn : ThingDefTable_Column
{
    public ThingDefTable_WeaponRangeColumn()
    {
        Label = "Range".Translate();
        Description = "Stat_Thing_Weapon_Range_Desc".Translate();
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var range = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new GenTable_NumCell(_range);
            }
        }

        return new GenTable_NumCell();
    }
}

public class ThingDefTable_CEReloadTimeColumn : ThingDefTable_StatDefColumn
{
    public ThingDefTable_CEReloadTimeColumn() : base(
        DefDatabase<StatDef>.GetNamed("ReloadTime")
    )
    {
        DiffMult = -1;
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (mcStat.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_StatCell(Stat, statReq);
        }

        return new GenTable_NumCell();
    }

    private static readonly StatDef mcStat = DefDatabase<StatDef>.GetNamed("MagazineCapacity");
}

public class ThingDefTable_CEMagCapColumn : ThingDefTable_StatDefColumn
{
    public ThingDefTable_CEMagCapColumn() : base(
        DefDatabase<StatDef>.GetNamed("MagazineCapacity")
    )
    {
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (Stat.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_NumCell(Stat.Worker.GetValue(statReq));
        }

        return new GenTable_NumCell();
    }
}

public class ThingDefTable_CEOneHandednessColumn : ThingDefTable_StatDefColumn
{
    public ThingDefTable_CEOneHandednessColumn() : base(
        DefDatabase<StatDef>.GetNamed("OneHandedness")
    )
    {
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (Stat.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_BoolCell(Stat.Worker.GetValue(statReq));
        }

        return new GenTable_NumCell();
    }
}