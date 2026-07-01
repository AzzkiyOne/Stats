using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.PlantDef;

public sealed class LightRequirementColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps.growMinGlow > 0f)
        {
            decimal cellValue = (100f * plantProps.growMinGlow).ToDecimal(0);

            return new NumberCell(cellValue, "0\\%");
        }

        return default;
    }
}
