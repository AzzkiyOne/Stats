using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerOutputPerFuelColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPowerTraderDefTableRecord,
            IRefuelableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.PowerCompProperties;
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (powerCompProps != null && refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            float powerOutput = powerCompProps.PowerConsumption * -1f;
            float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
            float powerOutputPerFuel = powerOutput / fuelConsumptionRate;

            return new NumberCell(powerOutputPerFuel, "0 W/u");
        }

        return default;
    }
}
