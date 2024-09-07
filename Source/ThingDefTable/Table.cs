using System.Collections.Generic;
using System.Linq;

namespace Stats.ThingDefTable;

internal class Table : GenTable.Table<ColumnDef, ThingAlike>
{
    public Table(
        List<ColumnDef> columns,
        List<ThingAlike> things
    ) : base(columns, things.Select(thing => new GenTable.Row<ThingAlike>(thing)).ToList())
    {
    }
}
