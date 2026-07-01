using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;
using Stats.Widgets_Legacy;

namespace Stats.Columns;

public abstract class ThingDefSetColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.String)
        where TCell :
            struct,
            IThingDefSetCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(Table tableWorker);

    private static readonly HashSet<Verse.ThingDef> _emptyThingDefHashSet = [];

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap, new ThingDefIcon(def))
            );
        Filter valueFieldFilter = new MTMFilter<Verse.ThingDef?>((int row) => this[row].Value ?? _emptyThingDefHashSet, valueFieldFilterOptions);
        // TODO: Figure out how to efficiently compare cells so that cells with equal values will be grouped together.
        int Compare(int row1, int row2) => row1.CompareTo(row2);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }
}
