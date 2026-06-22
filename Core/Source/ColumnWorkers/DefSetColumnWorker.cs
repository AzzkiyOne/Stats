using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class DefSetColumnWorker<TRecord, TCell>(ColumnDef def) :
    ColumnWorker<TRecord, TCell>(def, ColumnType.String)
        where TCell :
            struct,
            IDefSetCell
{
    protected abstract IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker);

    private static readonly HashSet<Verse.Def> _emptyDefHashSet = [];

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.Def?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        Filter valueFieldFilter = new MTMFilter<Verse.Def?>((int row) => this[row].Value ?? _emptyDefHashSet, valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(null, valueFieldFilter, CompareByCellText);

        return [valueField];
    }
}
