using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.PowerTraderDef;

public sealed class PowerOutputPerCellColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPowerTraderDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.PowerCompProperties;

        if (powerCompProps != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float area = thingDef.size.Area;
            float powerOutput = powerCompProps.PowerConsumption * -1f;
            float powerOutputPerCell = powerOutput / area;

            return new NumberCell(powerOutputPerCell, "0 W/c");
        }

        return default;
    }
}
