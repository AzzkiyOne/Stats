using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.AnimalDef;

public sealed class LeatherPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
