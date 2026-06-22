using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class AverageLitterSizeColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float averageLitterSize = AnimalProductionUtility.OffspringRange(thingDef).Average;

        return new NumberCell(averageLitterSize.ToDecimal(1), "0.0");
    }
}
