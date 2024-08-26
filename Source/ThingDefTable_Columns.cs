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

        list.Add(labelColumn.Id, labelColumn);
        list.Add(weaponRangeColumn.Id, weaponRangeColumn);

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
        float? minWidth = null,
        GenTable_ColumnType type = GenTable_ColumnType.Number
    ) : base(label, description, minWidth, type)
    {
        Id = id;
    }

    public abstract IGenTable_Cell GetCellData(ThingAlike thingAlike);
}

public class ThingDefTable_StatDefColumn : ThingDefTable_Column
{
    private readonly StatDef statDef;

    public ThingDefTable_StatDefColumn(StatDef statDef) : base(
        statDef.defName,
        statDef.LabelCap,
        statDef.description
    )
    {
        this.statDef = statDef;
    }

    public override IGenTable_Cell GetCellData(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        //if (statDef.Worker.ShouldShowFor(statReq) == false)
        //{
        //    return new GenTable_StatCell(
        //        statDef,
        //        float.NaN
        //    );
        //}

        return new GenTable_StatCell(
            statDef,
            statDef.Worker.GetValue(statReq)
        );
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

    public override IGenTable_Cell GetCellData(ThingAlike thing)
    {
        return new GenTable_StrCell()
        {
            ValueStr = thing.label,
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

    public override IGenTable_Cell GetCellData(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var range = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new GenTable_NumCell() { ValueNum = _range, ValueStr = _range.ToString() };
            }
        }

        return new GenTable_NumCell();
    }
}