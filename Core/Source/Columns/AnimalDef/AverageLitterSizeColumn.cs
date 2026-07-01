using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.AnimalDef;

public sealed class AverageLitterSizeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
