using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.ApparelDef.Reloadable;

public sealed class MaxChargesCountColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties != null)
        {
            decimal maxCharges = reloadableCompProperties.maxCharges;

            return new NumberCell(maxCharges);
        }

        return default;
    }
}
