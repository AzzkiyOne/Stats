using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;

namespace Stats.Columns.AnimalDef;

public sealed class GestationTimeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
