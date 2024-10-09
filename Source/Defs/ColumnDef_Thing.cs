﻿using System;

namespace Stats;

public sealed class ColumnDef_Thing : ColumnDef
{
    public Func<ThingAlike, ThingAlike?> prop = (ThingAlike thing) => thing;
    private ThingAlike? GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        if (GetValue(thing) is ThingAlike value)
        {
            return new CellWidget_Thing(value);
        }

        return null;
    }
}
