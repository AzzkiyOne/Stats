using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class GestationTimeColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float gestationTime = AnimalProductionUtility.GestationDaysLitter(thingDef);

        return new NumberCell(gestationTime.ToDecimal(1), "0.0 d");
    }
}
