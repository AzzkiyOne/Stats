using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class ColumnDef_ThingDefs : ColumnDef
{
    public Func<ThingAlike, List<ThingDef>> prop;
    private List<ThingDef> GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value.Count > 0)
        {
            return new CellWidget_ThingDefs(value);
        }

        return null;
    }
}
