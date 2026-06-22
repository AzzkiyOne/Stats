using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class FertilityRequirementColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.fertilityMin > 0f)
        {
            decimal cellValue = (100F * plantProps.fertilityMin).ToDecimal(1);

            return new NumberCell(cellValue, "0\\%");
        }

        return default;
    }
}
