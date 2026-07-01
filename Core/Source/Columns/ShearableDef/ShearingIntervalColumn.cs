using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.ShearableDef;

public sealed class ShearingIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IShearableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        if (shearableCompProps != null)
        {
            decimal shearIntervalDays = shearableCompProps.shearIntervalDays;

            return new NumberCell(shearIntervalDays, "0 d");
        }

        return default;
    }
}
