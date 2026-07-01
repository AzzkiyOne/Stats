using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.PlantDef;

public sealed class RawNutritionPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
