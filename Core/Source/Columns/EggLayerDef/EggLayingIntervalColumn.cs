using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.EggLayerDef;

public sealed class EggLayingIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
