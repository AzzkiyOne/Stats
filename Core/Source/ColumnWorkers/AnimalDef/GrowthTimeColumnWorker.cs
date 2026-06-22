using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class GrowthTimeColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

        return new NumberCell(growthTime, "0 d");
    }
}
