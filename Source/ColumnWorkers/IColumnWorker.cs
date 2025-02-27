namespace Stats;

public interface IColumnWorker
{
    ColumnCellStyle CellStyle { get; }
    ColumnDef ColumnDef { get; set; }
    ICellWidget? GetCellWidget(ThingRec thing);
    IFilterWidget GetFilterWidget();
    int Compare(ThingRec thing1, ThingRec thing2);
}

public interface IColumnWorker<ValueType> : IColumnWorker
{
    ValueType GetValue(ThingRec thing);
}
