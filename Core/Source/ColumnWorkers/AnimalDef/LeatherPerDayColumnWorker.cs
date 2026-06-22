using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class LeatherPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

        if (growthTime > 0f)
        {
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);
            float leatherPerDay = leatherAmount / growthTime;

            return new NumberCell(leatherPerDay.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
