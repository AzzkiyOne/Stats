using System.Collections.Generic;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class NumberColumnWorker<TRecord, TCell>(ColumnDef def) :
    ColumnWorker<TRecord, TCell>(def, ColumnType.Number)
        where TCell :
            struct,
            INumberCell
{
    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }
}
