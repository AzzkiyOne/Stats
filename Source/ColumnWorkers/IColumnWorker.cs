namespace Stats;

public interface IColumnWorker
{
    ColumnCellStyle CellStyle { get; }
    ColumnDef ColumnDef { get; set; }
    IWidget? GetTableCellContent(ThingRec thing);
    IWidget_FilterInput GetFilterWidget();
    int Compare(ThingRec thing1, ThingRec thing2);
}

public interface IColumnWorker<ValueType> : IColumnWorker
{
    ValueType GetValue(ThingRec thing);
}
