using Stats.Widgets;
using Stats.Widgets.Table.Filters;

namespace Stats;

public interface IColumnWorker
{
    ColumnCellStyle CellStyle { get; }
    ColumnDef ColumnDef { get; set; }
    IWidget? GetTableCellWidget(ThingRec thing);
    IWidget_FilterInput GetFilterWidget();
    int Compare(ThingRec thing1, ThingRec thing2);
}
