using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;
using UnityEngine;

namespace Stats.Columns;

public abstract class ThingDefColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.String)
        where TCell :
            struct,
            IThingDefCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(Table tableWorker);

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        Filter valueFieldFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].Value, valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(null, valueFieldFilter, CompareByCellText);

        return [valueField];
    }
}
