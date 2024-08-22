using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Stats;

static class Columns
{
    static public readonly Dictionary<string, Column<ThingAlike>> list = [];
    static Columns()
    {
        var labelColumn = new LabelColumn();
        var weaponRangeColumn = new WeaponRangeColumn();

        list.Add(labelColumn.id, labelColumn);
        list.Add(weaponRangeColumn.id, weaponRangeColumn);

        foreach (var statDef in DefDatabase<StatDef>.AllDefs)
        {
            var column = new StatDefColumn(statDef);

            list.TryAdd(column.id, column);
        }
    }
}

public abstract class Column<RowType>(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
) : IGenTableColumn<RowType>
{
    public string id { get; } = id;
    public string label { get; } = label ?? "";
    public string description { get; } = description ?? "";
    public float minWidth { get; } = minWidth ?? 100f;

    public abstract (
        string label,
        string? tip,
        Def? def,
        ThingDef? stuff
    ) GetCellDrawData(RowType row);
    public abstract int CompareRows(RowType r1, RowType r2);
}

public abstract class CachedColumn<RowType>(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
) : Column<RowType>(
    id,
    label,
    description,
    minWidth
)
{
    // Initialize in constructor with ThingAlikes count?
    private readonly Dictionary<RowType, Cell> cellsCache = [];

    private Cell GetCellFor(RowType row)
    {
        if (!cellsCache.ContainsKey(row))
        {
            cellsCache[row] = CreateCell(row);
        }

        return cellsCache[row];
    }

    public override (
        string label,
        string? tip,
        Def? def,
        ThingDef? stuff
    ) GetCellDrawData(RowType row)
    {
        var cell = GetCellFor(row);

        return (cell.ToString(), cell.tip, cell.def, cell.stuff);
    }
    public override int CompareRows(RowType r1, RowType r2)
    {
        return GetCellFor(r1).CompareTo(GetCellFor(r2));
    }

    protected abstract Cell CreateCell(RowType row);
}

public class StatDefColumn(
    StatDef statDef
) : CachedColumn<ThingAlike>(
    statDef.defName,
    statDef.LabelCap,
    statDef.description
)
{
    protected override Cell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return Cell.Empty;
        }

        float? valueAbs = null;
        string? valueString = null;
        //string? valueExplanation = null;

        // Maybe add some indication that there was an exception.
        try
        {
            valueAbs = statDef.Worker.GetValue(statReq);
        }
        catch
        {
        }

        if (valueAbs is float _valueAbs)
        {
            try
            {
                valueString = statDef.Worker.GetStatDrawEntryLabel(
                    statDef,
                    _valueAbs,
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
            //    valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Undefined, _valueAbs);
            //}
            //catch
            //{
            //}

            return new Cell(valueAbs, valueString);
        }

        return Cell.Empty;
    }
}

public class LabelColumn() : Column<ThingAlike>("Label", "Name", minWidth: 250f)
{
    public override (
        string label,
        string? tip,
        Def? def,
        ThingDef? stuff
    ) GetCellDrawData(ThingAlike row)
    {
        return (row.label, row.def.description, row.def, row.stuff);
    }
    public override int CompareRows(ThingAlike r1, ThingAlike r2)
    {
        return r1.label.CompareTo(r2.label);
    }
}

public class WeaponRangeColumn() : CachedColumn<ThingAlike>(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    protected override Cell CreateCell(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var value = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (value is float _value)
            {
                return new Cell(_value);
            }
        }

        return Cell.Empty;
    }
}