using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class LifeExpectancyColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        float lifeExpectancy = raceProperties.lifeExpectancy;

        return new NumberCell(lifeExpectancy, "0 y");
    }
}
