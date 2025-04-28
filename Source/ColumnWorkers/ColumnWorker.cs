using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker
{
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    public abstract TableColumnCellStyle CellStyle { get; }
    // "Widget?" is so the table can decide itself how to store/draw empty cells.
    public abstract Widget? GetTableCellWidget(ThingAlike thing);
    public abstract FilterWidget GetFilterWidget();
    public abstract int Compare(ThingAlike thing1, ThingAlike thing2);
}
