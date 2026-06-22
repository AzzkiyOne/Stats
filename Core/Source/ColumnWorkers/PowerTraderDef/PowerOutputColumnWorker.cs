using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.PowerTraderDef;

public sealed class PowerOutputColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPowerTraderDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.PowerCompProperties;

        if (powerCompProps is { PowerConsumption: < 0f })
        {
            float powerOutput = powerCompProps.PowerConsumption * -1f;

            return new NumberCell(powerOutput, "0 W");
        }

        return default;
    }
}
