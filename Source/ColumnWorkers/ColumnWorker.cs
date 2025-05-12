using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

// Column worker encapsulates a virtual property of a TObject (ThingAlike/GeneDef/etc).
//
// Column worker provides following abstractions:
// - GetTableCellWidget: how to display values of the property.
// - GetFilterWidget: what filter widget to use to filter by this property.
// - Compare: how to compare two property values.

public abstract class ColumnWorker
{
    // Why is this not in ColumnDef?
    //
    // This part of a column representation is governed by column worker,
    // because only column worker knows what type of data it encapsulates.
    public ColumnCellStyle CellStyle { get; }
    public ColumnDef ColumnDef { get; }
    protected ColumnWorker(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        ColumnDef = columnDef;
        CellStyle = cellStyle;
    }
}

public abstract class ColumnWorker<TObject> : ColumnWorker
{
    protected ColumnWorker(ColumnDef columnDef, ColumnCellStyle cellStyle) : base(columnDef, cellStyle)
    {
    }
    // "Widget?" is so the table can decide itself how to store/draw empty cells.
    public abstract Widget? GetTableCellWidget(TObject @object);
    // We pass IEnumerable<TObject> to this method, mainly so that if a column worker returns
    // one/many-to-many filter widget, it can generate a superset of all possible distinct
    // options for it.
    public abstract FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords);
    public abstract int Compare(TObject object1, TObject object2);
}
