using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public class Row
{
    public readonly ThingDef thingDef;
    // Storing cells like this is a potential memory leak.
    // If the set of columns cahnges for the same row
    // then new columns will create new cells.
    private readonly Dictionary<ColumnDef, ICell> cells = [];
    public Row(ThingDef thingDef)
    {
        this.thingDef = thingDef;
    }
    public ICell GetCell(ColumnDef column, Func<ThingDef, ICell> createCellIfNotFound)
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
