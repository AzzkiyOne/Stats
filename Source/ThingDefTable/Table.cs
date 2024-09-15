using System.Collections.Generic;
using System.Linq;

namespace Stats.ThingDefTable;

internal class Table : GenTable.Table<ThingAlike>
{
    public Table(
        List<GenTable.IColumn<ThingAlike>> columns,
        List<ThingAlike> things
    ) : base(columns, things.Select(thing => new GenTable.Row<ThingAlike>(thing, columns.Count)).ToList())
    {
    }
}
