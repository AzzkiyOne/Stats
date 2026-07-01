using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class MeatNutritionPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        RaceProperties raceProps = record.RaceProperties;
        Verse.ThingDef? meatDef = raceProps.meatDef;

        if (meatDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

            if (growthTime > 0f)
            {
                float meatNutrition = meatDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float meatAmount = AnimalProductionUtility.AdultMeatAmount(thingDef);
                float meatPerDay = meatAmount / growthTime;
                float meatNutritionPerDay = meatPerDay * meatNutrition;

                return new NumberCell(meatNutritionPerDay.ToDecimal(2), "0.00/d");
            }
        }

        return default;
    }
}
