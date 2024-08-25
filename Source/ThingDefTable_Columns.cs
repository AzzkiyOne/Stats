using System;
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

public abstract class ThingDefTable_Column(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null,
    GenTableColumnType type = GenTableColumnType.Number
) : GenTableColumn(label, description, minWidth, type)
{
    public string Id { get; } = id;

    public abstract IGenTableCell GetCellData(ThingAlike thingAlike);
}

public class ThingDefTable_StatDefColumn(StatDef statDef) : ThingDefTable_Column(
    statDef.defName,
    statDef.LabelCap,
    statDef.description
)
{
    public override IGenTableCell GetCellData(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        //if (statDef.Worker.ShouldShowFor(statReq) == false)
        //{
        //    return new GenTableNumCell();
        //}

        // It looks StatDefs have a default value of 1.
        float valueAbs = float.NaN;
        string valueString = "";
        //string valueExplanation = "";

        // Maybe add some indication that there was an exception.
        try
        {
            valueAbs = statDef.Worker.GetValue(statReq);
        }
        catch
        {
        }

        if (!float.IsNaN(valueAbs))
        {
            try
            {
                valueString = statDef.Worker.GetStatDrawEntryLabel(
                    statDef,
                    valueAbs,
                    ToStringNumberSense.Undefined,
                    statReq
                );
            }
            catch
            {
            }

            //try
            //{
            //    // Maybe we don't really need to cache explanation.
            //    // Because we only show one at a time.
            //    // The only issue is that it is shown at 60fps.
            //    valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Undefined, valueAbs);
            //}
            //catch
            //{
            //}
        }

        return new GenTableNumCell() { ValueNum = valueAbs, ValueStr = valueString };
    }
}

public class LabelColumn() : ThingDefTable_Column(
    "Label",
    "Name",
    minWidth: 250f,
    type: GenTableColumnType.String
)
{
    public override IGenTableCell GetCellData(ThingAlike thing)
    {
        return new GenTableStrCell()
        {
            ValueStr = thing.label,
            Tip = thing.def.description,
            Def = thing.def,
            Stuff = thing.stuff,
        };
    }
}

public class ThingDefTable_WeaponRangeColumn() : ThingDefTable_Column(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    public override IGenTableCell GetCellData(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var range = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new GenTableNumCell() { ValueNum = _range, ValueStr = _range.ToString() };
            }
        }

        return new GenTableNumCell();
    }
}