using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerOutputColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
