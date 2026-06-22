using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class LifeSpanColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.LifespanDays > 0f)
        {
            decimal cellValue = plantProps.LifespanDays.ToDecimal(1);

            return new NumberCell(cellValue, "0.0 d");
        }

        return default;
    }
}
