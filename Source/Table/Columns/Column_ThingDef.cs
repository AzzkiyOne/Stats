using System;
using Verse;

namespace Stats.Table.Columns;

public sealed class Column_ThingDef : Column
{
    public Func<ThingAlike, ThingDef?> prop;
    private ThingDef? GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICell? GetCell(ThingAlike thing)
    {
        if (GetValue(thing) is ThingDef value)
        {
            return new Cells.Cell_Gen<string>(
                value.LabelCap,
                value.LabelCap,
                value.description,
                new ThingIcon(value),
                new ThingAlike(value)
            );
        }

        return null;
    }
}
