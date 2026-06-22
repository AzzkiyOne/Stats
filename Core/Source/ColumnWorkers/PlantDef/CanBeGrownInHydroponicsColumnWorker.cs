using Stats.TableRecords;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class CanBeGrownInHydroponicsColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.PlantProperties.sowTags.Contains("Hydroponic");
    }
}
