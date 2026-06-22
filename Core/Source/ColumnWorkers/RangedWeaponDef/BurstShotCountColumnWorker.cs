using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class BurstShotCountColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;

        if (verbProps is { Ranged: true, showBurstShotStats: true })
        {
            decimal burstShotCount = verbProps.burstShotCount;

            return new NumberCell(burstShotCount);
        }

        return default;
    }
}
