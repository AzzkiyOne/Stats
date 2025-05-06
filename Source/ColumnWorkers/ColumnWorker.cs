using System.Collections.Generic;
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
    // We pass IEnumerable<TObject> to this method, mainly so that if a column worker returns
    // one/many-to-many filter widget, it can generate a superset of all possible distinct
    // options for it.
    public abstract FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords);
    public abstract int Compare(TObject object1, TObject object2);
}
