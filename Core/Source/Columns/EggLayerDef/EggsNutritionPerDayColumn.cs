using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsNutritionPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IEggLayerDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;
            float eggsNutritionPerDay = eggsPerDay * eggNutrition;

            return new NumberCell(eggsNutritionPerDay.ToDecimal(2), "0.00/d");
        }

        return default;
    }
}
