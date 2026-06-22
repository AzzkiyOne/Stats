using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class NuzzleIntervalColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        RaceProperties raceProps = record.RaceProperties;
        float nuzzleInterval = raceProps.nuzzleMtbHours;

        if (nuzzleInterval > 0f)
        {
            return new NumberCell(nuzzleInterval.ToDecimal(1), "0.0 h");
        }

        return default;
    }
}
