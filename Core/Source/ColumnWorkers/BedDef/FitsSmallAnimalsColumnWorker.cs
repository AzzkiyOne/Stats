using Stats.TableRecords;

namespace Stats.ColumnWorkers.BedDef;

public sealed class FitsSmallAnimalsColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IBuildingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.BuildingProperties.bed_humanlike == false;
    }
}
