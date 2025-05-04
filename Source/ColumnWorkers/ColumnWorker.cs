using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker<T>
{
    public TableColumnCellStyle CellStyle { get; }
    protected ColumnWorker(TableColumnCellStyle cellStyle)
    {
        CellStyle = cellStyle;
    }
    // "Widget?" is so the table can decide itself how to store/draw empty cells.
    public abstract Widget? GetTableCellWidget(T thing);
    public abstract FilterWidget<T> GetFilterWidget();
    public abstract int Compare(T thing1, T thing2);
}
