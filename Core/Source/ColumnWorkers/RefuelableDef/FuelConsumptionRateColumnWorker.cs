using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.RefuelableDef;

public sealed class FuelConsumptionRateColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRefuelableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps != null)
        {
            // TODO: Difficulty scaling.
            float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;

            return new NumberCell(fuelConsumptionRate.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
