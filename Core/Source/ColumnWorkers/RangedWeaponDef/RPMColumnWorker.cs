using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class RPMColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;

        if (verbProps is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            // Reminder: This is not IRL RPM.
            float secondsBetweenShots = verbProps.ticksBetweenBurstShots.TicksToSeconds();
            float rpm = 60f / secondsBetweenShots;

            return new NumberCell(rpm, "0 rpm");
        }

        return default;
    }
}
