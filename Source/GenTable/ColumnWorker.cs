namespace Stats.GenTable;

public abstract class ColumnWorker<DataType>
{
    public abstract ICell? GetCell(DataType data);
}
