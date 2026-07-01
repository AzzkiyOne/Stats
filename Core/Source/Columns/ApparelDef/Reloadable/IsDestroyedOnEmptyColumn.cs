using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class IsDestroyedOnEmptyColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        return reloadableCompProperties?.destroyOnEmpty == true;
    }
}
