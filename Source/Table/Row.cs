using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

class Row
{
    public readonly ThingDef thingDef;
    private readonly Dictionary<Column, ICell> cells = [];
    public Row(ThingDef thingDef)
    {
        this.thingDef = thingDef;
    }
    public ICell GetCell(Column column, Func<ThingDef, ICell> createCellIfNotFound)
    {
        cells.TryGetValue(column, out ICell cell);

        if (cell == null)
        {
            return cells[column] = createCellIfNotFound(thingDef);
        }
        else
        {
            return cells[column];
        }
    }
}
