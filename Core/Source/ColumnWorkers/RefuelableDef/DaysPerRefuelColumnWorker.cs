using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.RefuelableDef;

public sealed class DaysPerRefuelColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRefuelableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            float fuelCapacity = refuelableCompProps.fuelCapacity;
            float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
            float daysPerRefuel = fuelCapacity / fuelConsumptionRate;

            return new NumberCell(daysPerRefuel.ToDecimal(1), "0.0 d");
        }

        return default;
    }
}
