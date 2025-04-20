using Stats.Widgets;
using Stats.Widgets.Table.Filters.Widgets;

namespace Stats.ColumnWorkers;

public interface IColumnWorker
{
    ColumnCellStyle CellStyle { get; }
    ColumnDef ColumnDef { get; set; }
    IWidget? GetTableCellWidget(ThingAlike thing);
    IFilterWidget GetFilterWidget();
    int Compare(ThingAlike thing1, ThingAlike thing2);
}
