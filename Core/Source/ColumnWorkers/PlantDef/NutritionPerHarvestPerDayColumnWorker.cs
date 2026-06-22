using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class NutritionPerHarvestPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        // TODO: This is mostly copy paste from NutritionPerHarvestColumnWorker.
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;
            decimal cellValue = (nutritionPerHarvest / plantProps.GetGrowDaysActual()).ToDecimal(3);

            return new NumberCell(cellValue, "0.000/d");
        }

        return default;
    }
}
