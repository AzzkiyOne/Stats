using System;

namespace Stats.GenTable;

public interface IColumnWorker<DataType>
{
    public bool ShouldShowFor(DataType dataType);
    public string GetCellText(DataType data);
    public string GetCellTip(DataType data);
    public IComparable GetCellSortValue(DataType data);
}