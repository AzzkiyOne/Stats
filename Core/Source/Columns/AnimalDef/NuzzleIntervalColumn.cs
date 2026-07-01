using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class NuzzleIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
