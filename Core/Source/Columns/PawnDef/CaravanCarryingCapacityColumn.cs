using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class CaravanCarryingCapacityColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        float baseBodySize = raceProperties.baseBodySize;
        float caravanCarryingCapacity = baseBodySize * MassUtility.MassCapacityPerBodySize;

        return new NumberCell(caravanCarryingCapacity, "0 kg");
    }
}
