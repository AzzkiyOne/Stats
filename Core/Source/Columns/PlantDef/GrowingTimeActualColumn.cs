using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.PlantDef;

public sealed class GrowingTimeActualColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.growDays > 0f)
        {
            decimal cellValue = plantProps.GetGrowDaysActual().ToDecimal(1);

            return new NumberCell(cellValue, "0.0 d");
        }

        return default;
    }
}
