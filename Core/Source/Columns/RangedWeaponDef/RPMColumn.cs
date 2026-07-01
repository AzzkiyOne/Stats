using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class RPMColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
