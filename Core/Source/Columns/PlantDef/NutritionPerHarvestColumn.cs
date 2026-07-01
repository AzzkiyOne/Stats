using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.PlantDef;

public sealed class NutritionPerHarvestColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.harvestedThingDef != null)
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return new NumberCell(nutritionPerHarvest.ToDecimal(2), "0.00");
        }

        return default;
    }
}
