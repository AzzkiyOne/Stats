using System.Collections.Generic;
using Verse;

namespace Stats;

public abstract class ColumnWorker<CellType> : IColumnWorker<CellType> where CellType : ICellWidget
{
    public ColumnDef ColumnDef { get; set; }
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellType?
    > Cells = [];
    protected abstract CellType? CreateCell(ThingDef thingDef, ThingDef? stuffDef);
    public ICellWidget? GetCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            Cells[key] = cell = CreateCell(thingDef, stuffDef);
        }

        return cell;
    }
}
