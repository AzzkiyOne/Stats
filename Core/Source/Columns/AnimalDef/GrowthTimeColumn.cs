using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class GrowthTimeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
