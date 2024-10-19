using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public abstract class ColumnWorker<CellType> : IColumnWorker where CellType : ICellWidget
{
    public ColumnDef ColumnDef { get; set; }
    private readonly Dictionary<ThingRec, CellType?> Cells = [];
    protected abstract CellType? CreateCell(ThingRec thing);
    protected CellType? GetCell(ThingRec thing)
    {
        var exists = Cells.TryGetValue(thing, out var cell);

        if (exists == false)
        {
            Cells[thing] = cell = CreateCell(thing);
        }

        return cell;
    }
    public void DrawCell(Rect targetRect, ThingRec thing)
    {
        GetCell(thing)?.Draw(targetRect);
    }
    public float? GetCellMinWidth(ThingRec thing)
    {
        return GetCell(thing)?.MinWidth;
    }
    public virtual IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Str(thing => GetCell(thing)?.ToString());
    }
    public int Compare(ThingRec thing1, ThingRec thing2)
    {
        var c1 = GetCell(thing1);
        var c2 = GetCell(thing2);

        if (c1 == null)
        {
            if (c2 == null)
            {
                return 0;
            }

            return -1;
        }

        return c1.CompareTo(c2);
    }
}
