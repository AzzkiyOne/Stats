using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.EggLayerDef;

public sealed class EggLayingIntervalColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IEggLayerDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps != null)
        {
            float eggLayingInterval = eggLayerCompProps.eggLayIntervalDays;

            return new NumberCell(eggLayingInterval.ToDecimal(1), "0.0 d");
        }

        return default;
    }
}
