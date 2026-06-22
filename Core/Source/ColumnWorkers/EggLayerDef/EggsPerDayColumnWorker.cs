using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.EggLayerDef;

public sealed class EggsPerDayColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IEggLayerDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            float eggLayIntervalDays = eggLayerCompProps.eggLayIntervalDays;
            float averageEggCount = eggLayerCompProps.eggCountRange.Average;
            float eggsPerDay = averageEggCount / eggLayIntervalDays;

            return new NumberCell(eggsPerDay.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
