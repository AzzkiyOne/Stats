using Stats.TableRecords;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class IsDestroyedOnHarvestColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.PlantProperties.HarvestDestroys;
    }
}
