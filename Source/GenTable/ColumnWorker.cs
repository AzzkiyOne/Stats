using System;

namespace Stats.GenTable;

public interface IColumnWorker<DataType>
{
    public bool ShouldShowFor(DataType data);
    public string GetCellText(DataType data);
    public string GetCellTip(DataType data);
    public IComparable GetCellSortValue(DataType data);
    public DefReference? GetDefRef(DataType data);
}