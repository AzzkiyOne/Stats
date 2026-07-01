using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.MilkableDef;

public sealed class MilkingIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps != null)
        {
            decimal milkIntervalDays = milkableCompProps.milkIntervalDays;

            return new NumberCell(milkIntervalDays, "0 d");
        }

        return default;
    }
}
