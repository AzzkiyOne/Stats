using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public interface IColumnWorker
{
    TableColumnCellStyle CellStyle { get; }
    ColumnDef ColumnDef { get; set; }
    IWidget? GetTableCellWidget(ThingAlike thing);
    IFilterWidget GetFilterWidget();
    int Compare(ThingAlike thing1, ThingAlike thing2);
}
