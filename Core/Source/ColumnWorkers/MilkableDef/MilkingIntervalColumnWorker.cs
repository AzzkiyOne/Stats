using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.MilkableDef;

public sealed class MilkingIntervalColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
