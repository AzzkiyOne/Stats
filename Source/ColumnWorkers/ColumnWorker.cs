using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker
{
    public TableColumnCellStyle CellStyle { get; }
    protected ColumnWorker(TableColumnCellStyle cellStyle)
    {
        CellStyle = cellStyle;
    }
    // "Widget?" is so the table can decide itself how to store/draw empty cells.
    public abstract Widget? GetTableCellWidget(ThingAlike thing);
    public abstract FilterWidget GetFilterWidget();
    public abstract int Compare(ThingAlike thing1, ThingAlike thing2);
}
