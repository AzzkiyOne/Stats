using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.ShearableDef;

public sealed class WoolPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IShearableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            float woolAmount = shearableCompProps.woolAmount;
            float shearIntervalDays = shearableCompProps.shearIntervalDays;
            float woolPerDay = woolAmount / shearIntervalDays;

            return new NumberCell(woolPerDay.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
