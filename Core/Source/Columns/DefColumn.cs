using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;

namespace Stats.Columns;

public abstract class DefColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.String)
        where TCell :
            struct,
            IDefCell
{
    protected abstract IEnumerable<Verse.Def?> GetValueFieldFilterOptions(Table tableWorker);

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.Def?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        Filter valueFieldFilter = new OTMFilter<Verse.Def?>((int row) => this[row].Value, valueFieldFilterOptions);
        int CompareByDefLabel(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(null, valueFieldFilter, CompareByDefLabel);

        return [valueField];
    }
}
