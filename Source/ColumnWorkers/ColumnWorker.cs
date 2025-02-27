using System.Collections.Generic;

namespace Stats;

public abstract class ColumnWorker<ValueType> : IColumnWorker<ValueType>
{
    public abstract ColumnCellStyle CellStyle { get; }
    public ColumnDef ColumnDef { get; set; }
    private readonly Dictionary<ThingRec, ICellWidget?> Cells = [];
    public abstract ValueType GetValue(ThingRec thing);
    protected virtual bool ShouldShowValue(ValueType value)
    {
        return value != null;
    }
    protected abstract ICellWidget ValueToCellWidget(ValueType value, ThingRec thing);
    public ICellWidget? GetCellWidget(ThingRec thing)
    {
        var exists = Cells.TryGetValue(thing, out var cell);

        if (exists == false)
        {
            try
            {
                var value = GetValue(thing);

                if (ShouldShowValue(value))
                {
                    Cells[thing] = cell = ValueToCellWidget(value, thing);
                }
                else
                {
                    Cells[thing] = null;
                }
            }
            catch
            {
                Cells[thing] = null;
            }
        }

        return cell;
    }
    public abstract IFilterWidget GetFilterWidget();
    public abstract int Compare(ThingRec thing1, ThingRec thing2);
}
