using Stats.Widgets;

namespace Stats;

public abstract class ColumnWorker<TObject>
{
    public TableColumnCellStyle CellStyle { get; }
    protected ColumnWorker(TableColumnCellStyle cellStyle)
    {
        CellStyle = cellStyle;
    }
    // "Widget?" is so the table can decide itself how to store/draw empty cells.
    public abstract Widget? GetTableCellWidget(TObject @object);
    public abstract FilterWidget<TObject> GetFilterWidget();
    public abstract int Compare(TObject object1, TObject object2);
}
