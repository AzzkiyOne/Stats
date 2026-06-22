using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.MilkableDef;

public sealed class MilkNutritionPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float milkAmount = milkableCompProps.milkAmount;
            float milkIntervalDays = milkableCompProps.milkIntervalDays;
            float milkPerDay = milkAmount / milkIntervalDays;
            float milkNutritionPerDay = milkPerDay * milkNutrition;

            return new NumberCell(milkNutritionPerDay.ToDecimal(2), "0.00/d");
        }

        return default;
    }
}
