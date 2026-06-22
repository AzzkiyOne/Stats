using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers;

public abstract class ThingDefColumnWorker<TRecord, TCell>(ColumnDef def) :
    ColumnWorker<TRecord, TCell>(def, ColumnType.String)
        where TCell :
            struct,
            IThingDefCell
{
    public override bool ShouldDrawCellsNow => Event.current is { type: EventType.Repaint or EventType.MouseDown or EventType.MouseUp };

    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
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
