using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.RefuelableDef;

public sealed class FuelCapacityColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IRefuelableDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                float fuelCapacity = refuelableCompProps.fuelCapacity;

                return new ThingDefCountCell(fuelType, fuelCapacity);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }
}
