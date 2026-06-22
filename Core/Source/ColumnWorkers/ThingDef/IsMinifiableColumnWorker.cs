using Stats.TableRecords;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class IsMinifiableColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.ThingDef.Minifiable;
    }
}
