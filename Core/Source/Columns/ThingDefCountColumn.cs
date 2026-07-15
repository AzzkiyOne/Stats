using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;
using UnityEngine;

namespace Stats.Columns;

public abstract class ThingDefCountColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.Number)
        where TCell :
            struct,
            IThingDefCountCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker);

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        Filter countFilter = new NumberFilter((int row) => this[row].Count);
        int CompareByCount(int row1, int row2) => this[row1].Count.CompareTo(this[row2].Count);
        CellField countField = new("Amount", countFilter, CompareByCount);

        IEnumerable<NTMFilterOption<Verse.ThingDef?>> thingDefFilterOptions = GetTypeFieldFilterOptions(tableWorker)
            .OrderBy(thingDef => thingDef?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        Filter thingDefFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].ThingDef, thingDefFilterOptions);
        int CompareByThingDefLabel(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].ThingDefLabel, this[row2].ThingDefLabel);
        CellField thingDefField = new("Type", thingDefFilter, CompareByThingDefLabel);

        return [countField, thingDefField];
    }
}
