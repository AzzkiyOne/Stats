using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.PawnDef;

public sealed class LifeExpectancyColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
