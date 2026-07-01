using Stats.TableRecords;

namespace Stats.Columns.ThingDef;

public sealed class HasInteractionCellColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.ThingDef.hasInteractionCell;
    }
}
