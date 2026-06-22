using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class HarvestYieldColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            decimal yield = Mathf.CeilToInt(plantProps.harvestYield);

            return new ThingDefCountCell(plantProps.harvestedThingDef, yield);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();
    }
}
