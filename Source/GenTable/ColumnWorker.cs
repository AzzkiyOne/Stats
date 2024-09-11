namespace Stats.GenTable;

public abstract class ColumnWorker<DataType>
{
    public abstract Cell? GetCell(DataType data);
}
