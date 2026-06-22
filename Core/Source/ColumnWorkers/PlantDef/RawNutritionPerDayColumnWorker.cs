using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class RawNutritionPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.growDays > 0f)
        {
            float nutrition = record.ThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            decimal cellValue = (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);

            return new NumberCell(cellValue, "0.000/d");
        }

        return default;
    }
}
