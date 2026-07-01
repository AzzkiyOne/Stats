using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.PlantDef;

public sealed class LifeSpanColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
