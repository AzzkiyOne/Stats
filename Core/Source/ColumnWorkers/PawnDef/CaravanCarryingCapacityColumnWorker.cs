using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.PawnDef;

public sealed class CaravanCarryingCapacityColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
