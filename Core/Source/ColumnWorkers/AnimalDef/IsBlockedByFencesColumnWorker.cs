using Stats.TableRecords;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class IsBlockedByFencesColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.RaceProperties.FenceBlocked;
    }
}
