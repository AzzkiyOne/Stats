using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.ShearableDef;

public sealed class WoolAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IShearableDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        if (shearableCompProps != null)
        {
            Verse.ThingDef woolDef = shearableCompProps.woolDef;
            decimal woolAmount = shearableCompProps.woolAmount;

            return new ThingDefCountCell(woolDef, woolAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();
    }
}
