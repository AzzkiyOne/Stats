using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.MilkableDef;

public sealed class MilkAmountColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps != null)
        {
            Verse.ThingDef milkDef = milkableCompProps.milkDef;
            decimal milkAmount = milkableCompProps.milkAmount;

            return new ThingDefCountCell(milkDef, milkAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();
    }
}
