using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using Stats.Utils.Extensions;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountCell>(columnDef)
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

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();
    }
}
