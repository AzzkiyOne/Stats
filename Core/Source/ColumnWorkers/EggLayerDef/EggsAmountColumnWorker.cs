using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.EggLayerDef;

public sealed class EggsAmountColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IEggLayerDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggAmount = eggLayerCompProps.eggCountRange.Average;

            return new ThingDefCountCell(eggDef, eggAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();
    }
}
