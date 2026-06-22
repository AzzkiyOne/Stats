using RimWorld;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.BedDef;

public sealed class FitsLargeAnimalsColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
        where TRecord :
            IBuildingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        BuildingProperties buildingProperties = record.BuildingProperties;

        return buildingProperties is { bed_humanlike: false, bed_maxBodySize: > 0.55f };
    }
}
