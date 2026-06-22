using RimWorld;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.ApparelDef.Reloadable;

public sealed class IsDestroyedOnEmptyColumnWorker<TRecord>(ColumnDef columnDef) :
    BooleanColumnWorker<TRecord>(columnDef)
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
