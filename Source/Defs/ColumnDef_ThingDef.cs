using System;
using Verse;

namespace Stats;

public sealed class ColumnDef_ThingDef : ColumnDef
{
    public Func<ThingAlike, ThingDef?> prop;
    private ThingDef? GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        if (GetValue(thing) is ThingDef value)
        {
            return new CellWidget_Gen<string>(
                value.LabelCap,
                value.LabelCap,
                value.description,
                new ThingIconWidget(value),
                new ThingAlike(value)
            );
        }

        return null;
    }
}
