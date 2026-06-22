using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class ProductPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.growDays > 0f)
        {
            decimal cellValue = (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);

            return new NumberCell(cellValue, "0.00/d");
        }

        return default;
    }
}
