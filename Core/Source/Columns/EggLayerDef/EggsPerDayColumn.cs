using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
