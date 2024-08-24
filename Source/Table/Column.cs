using System;
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
    float? minWidth = null,
    bool isComparable = true
) : IGenTableColumn<RowType>
{
    public string id { get; } = id;
    public string label { get; } = label ?? "";
    public string description { get; } = description ?? "";
    public float minWidth { get; } = minWidth ?? 100f;
    public bool isComparable { get; } = isComparable;

    public abstract GenTableCellDrawData GetCellDrawData(RowType row);
    public abstract int CompareRows(RowType r1, RowType r2);
}

public class Cache<K, V>(Func<K, V> factory)
{
    private readonly Dictionary<K, V> dict = new();
    private readonly Func<K, V> factory = factory;

    public V GetValue(K key)
    {
        var containsValue = dict.TryGetValue(key, out V value);

        if (!containsValue)
        {
            var newValue = factory(key);

            dict[key] = newValue;

            return newValue;
        }

        return value;
    }
}

public class StatDefColumn : Column<ThingAlike>
{
    private readonly StatDef statDef;
    private readonly Cache<ThingAlike, (float, string)> cache;

    public StatDefColumn(StatDef statDef) : base(
        statDef.defName,
        statDef.LabelCap,
        statDef.description
    )
    {
        this.statDef = statDef;
        cache = new(CreateCell);
    }

    private (float, string) CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return (float.NaN, "");
        }

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

        return (valueAbs, valueString);
    }

    public override GenTableCellDrawData GetCellDrawData(ThingAlike row)
    {
        var cell = cache.GetValue(row);

        return (cell.Item2, "", null, null);
    }
    public override int CompareRows(ThingAlike r1, ThingAlike r2)
    {
        return cache.GetValue(r1).Item1.CompareTo(cache.GetValue(r2).Item1);
    }
}

public class LabelColumn() : Column<ThingAlike>(
    "Label",
    "Name",
    minWidth: 250f,
    isComparable: false
)
{
    public override GenTableCellDrawData GetCellDrawData(ThingAlike row)
    {
        return (row.label, row.def.description, row.def, row.stuff);
    }
    public override int CompareRows(ThingAlike r1, ThingAlike r2)
    {
        return r1.label.CompareTo(r2.label);
    }
}

public class WeaponRangeColumn : Column<ThingAlike>
{
    private readonly Cache<ThingAlike, (float, string)> cache;

    public WeaponRangeColumn() : base(
        "WeaponRange",
        "Range".Translate(),
        "Stat_Thing_Weapon_Range_Desc".Translate()
    )
    {
        cache = new(CreateCell);
    }

    private (float, string) CreateCell(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var range = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return (_range, _range.ToString());
            }
        }

        return (float.NaN, "");
    }

    public override GenTableCellDrawData GetCellDrawData(ThingAlike row)
    {
        var cell = cache.GetValue(row);

        return (cell.Item2, "", null, null);
    }
    public override int CompareRows(ThingAlike r1, ThingAlike r2)
    {
        return cache.GetValue(r1).Item1.CompareTo(cache.GetValue(r2).Item1);
    }
}