using System.Linq;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

public class Column_Stat : GenTable.ColumnDef, GenTable.IColumn<ThingAlike>, IFilterProvider<ThingAlike>
{
    public StatDef stat;
    public bool formatValue = true;
    private float? GetValue(ThingAlike thing)
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

            //if (Column.Type == GenTable.ColumnType.Boolean)
            //{
            //    return new GenTable.Cell_Bool(statValue_num);
            //}

            return new GenTable.Cell_Num(_statValue_num, statValue_str);
        }

        return null;
    }
    public IFilter<ThingAlike> GetFilter()
    {
        return new FilterNum<ThingAlike>(this, GetValue);
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

public class Column_WeaponRange : GenTable.ColumnDef, GenTable.IColumn<ThingAlike>, IFilterProvider<ThingAlike>
{
    private float? GetValue(ThingAlike thing)
    {
        if (thing.Def.IsRangedWeapon && thing.Def.Verbs.Count > 0)
        {
            var range = thing.Def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return _range;
            }
        }

        return null;
    }
    public GenTable.ICell? GetCell(ThingAlike thing)
    {
        var value = GetValue(thing);

        return value is float _value ? new GenTable.Cell_Num(_value) : null;
    }
    public IFilter<ThingAlike> GetFilter()
    {
        return new FilterNum<ThingAlike>(this, GetValue);
    }
}