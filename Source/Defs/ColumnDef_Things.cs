using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class ColumnDef_Things : ColumnDef
{
    public Func<ThingAlike, List<ThingAlike>> prop;
    private List<ThingAlike> GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value.Count > 0)
        {
            return new CellWidget_Things(value);
        }

        return null;
    }
}
