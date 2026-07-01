using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class MaxChargesCountColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
