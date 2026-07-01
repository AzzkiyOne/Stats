using System.Collections.Generic;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;

namespace Stats.Columns;

public abstract class NumberColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.Number)
        where TCell :
            struct,
            INumberCell
{
    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        Filter valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }
}
