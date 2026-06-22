using Stats.TableRecords;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class HasInteractionCellColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.ThingDef.hasInteractionCell;
    }
}
