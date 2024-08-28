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
        var labelColumn = new LabelColumn();
        var weaponRangeColumn = new ThingDefTable_WeaponRangeColumn();
        var ceReloadTimeColumn = new ThingDefTable_CEReloadTimeColumn();
        var ceMagCapColumn = new ThingDefTable_CEMagCapColumn();

        list.Add(labelColumn.Id, labelColumn);
        list.Add(weaponRangeColumn.Id, weaponRangeColumn);
        list.Add(ceReloadTimeColumn.Id, ceReloadTimeColumn);
        list.Add(ceMagCapColumn.Id, ceMagCapColumn);

        foreach (var statDef in DefDatabase<StatDef>.AllDefs)
        {
            var column = new ThingDefTable_StatDefColumn(statDef);

            list.TryAdd(column.Id, column);
        }
    }
}

public abstract class ThingDefTable_Column : GenTable_Column
{
    public string Id { get; }

    public ThingDefTable_Column(
        string id,
        string? label = null,
        string? description = null,
        float minWidth = 75f,
        GenTable_ColumnType type = GenTable_ColumnType.Number
    ) : base(label, description, minWidth, type)
    {
        Id = id;
    }

    public abstract IGenTable_Cell CreateCell(ThingAlike thingAlike);
}

public class ThingDefTable_StatDefColumn : ThingDefTable_Column
{
    protected readonly StatDef statDef;

    public ThingDefTable_StatDefColumn(StatDef statDef) : base(
        statDef.defName,
        statDef.LabelCap,
        statDef.description
    )
    {
        this.statDef = statDef;
        DiffMult = statDef.GetModExtension<StatDefExtension>()?.diffMult ?? 1;
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (statDef.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_StatCell(statDef, statReq);
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
    public LabelColumn() : base(
        "Label",
        "Name",
        minWidth: 250f,
        type: GenTable_ColumnType.String
    )
    {
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
    public ThingDefTable_WeaponRangeColumn() : base(
        "WeaponRange",
        "Range".Translate(),
        "Stat_Thing_Weapon_Range_Desc".Translate()
    )
    {
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
            return new GenTable_StatCell(statDef, statReq);
        }

        return new GenTable_NumCell();
    }

    private static readonly StatDef mcStat = DefDatabase<StatDef>.GetNamed("MagazineCapacity");
}

public class ThingDefTable_CEMagCapColumn : ThingDefTable_Column
{
    public ThingDefTable_CEMagCapColumn() : base(
        stat.defName,
        stat.LabelCap,
        stat.description
    )
    {
    }

    public override IGenTable_Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (stat.Worker.ShouldShowFor(statReq))
        {
            return new GenTable_NumCell(stat.Worker.GetValue(statReq));
        }

        return new GenTable_NumCell();
    }

    private static readonly StatDef stat = DefDatabase<StatDef>.GetNamed("MagazineCapacity");
}
