using System;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

public class Column_Stat : GenTable.ColumnDef, GenTable.IColumn<ThingAlike>, IFilterProvider<ThingAlike>
{
    public StatDef stat;
    public bool formatValue = true;
    protected virtual float? GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (stat.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        return stat.Worker.GetValue(statReq);
    }
    public virtual GenTable.ICell? GetCell(ThingAlike thing)
    {
        var statValue_num = GetValue(thing);

        if (statValue_num is float _statValue_num)
        {
            string? statValue_str = null;

            if (formatValue)
            {
                statValue_str = stat.Worker.GetStatDrawEntryLabel(
                    stat,
                    _statValue_num,
                    ToStringNumberSense.Absolute,
                    // This is necessary (despite statReq being "optional") because some mods
                    // override this signature.
                    StatRequest.For(thing.Def, thing.Stuff)
                );
            }

            if (Type == GenTable.ColumnType.Boolean)
            {
                return new GenTable.Cell_Bool(_statValue_num);
            }

            return new GenTable.Cell_Num(_statValue_num, statValue_str);
        }

        return null;
    }
    public IFilter<ThingAlike> GetFilter()
    {
        if (Type == GenTable.ColumnType.Boolean)
        {
            return new Filter_Bool<ThingAlike>(this, (t) => GetValue(t) > 0f);
        }

        return new Filter_Num<ThingAlike>(this, GetValue);
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

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

public class Column_Num : GenTable.ColumnDef, GenTable.IColumn<ThingAlike>, IFilterProvider<ThingAlike>
{
    public Func<ThingAlike, float?> getValue;
    public GenTable.ICell? GetCell(ThingAlike thing)
    {
        var value = getValue(thing);

        return value is float _value ? new GenTable.Cell_Num(_value) : null;
    }
    public IFilter<ThingAlike> GetFilter()
    {
        return new Filter_Num<ThingAlike>(this, getValue);
    }
}

public class Column_Bool : GenTable.ColumnDef, GenTable.IColumn<ThingAlike>, IFilterProvider<ThingAlike>
{
    public Func<ThingAlike, bool?> getValue;
    public GenTable.ICell? GetCell(ThingAlike thing)
    {
        var value = getValue(thing);

        return value is bool _value ? new GenTable.Cell_Bool(_value) : null;
    }
    public IFilter<ThingAlike> GetFilter()
    {
        return new Filter_Bool<ThingAlike>(this, getValue);
    }
}
