using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.ShearableDef;

public sealed class ShearingIntervalColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IShearableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        if (shearableCompProps != null)
        {
            decimal shearIntervalDays = shearableCompProps.shearIntervalDays;

            return new NumberCell(shearIntervalDays, "0 d");
        }

        return default;
    }
}
