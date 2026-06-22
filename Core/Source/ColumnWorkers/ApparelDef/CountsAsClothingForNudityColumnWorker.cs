using Stats.TableRecords;

namespace Stats.ColumnWorkers.ApparelDef;

public sealed class CountsAsClothingForNudityColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IApparelDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.ApparelProperties.countsAsClothingForNudity;
    }
}
