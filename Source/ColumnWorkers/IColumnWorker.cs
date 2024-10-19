using UnityEngine;

namespace Stats;

public interface IColumnWorker
{
    ColumnDef ColumnDef { get; set; }
    void DrawCell(Rect targetRect, ThingRec thing);
    float? GetCellMinWidth(ThingRec thing);
    IFilterWidget GetFilterWidget();
    int Compare(ThingRec thing1, ThingRec thing2);
}
