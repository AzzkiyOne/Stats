using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.MilkableDef;

public sealed class MilkPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps is { milkIntervalDays: > 0 })
        {
            float milkAmount = milkableCompProps.milkAmount;
            float milkIntervalDays = milkableCompProps.milkIntervalDays;
            float milkPerDay = milkAmount / milkIntervalDays;

            return new NumberCell(milkPerDay.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
