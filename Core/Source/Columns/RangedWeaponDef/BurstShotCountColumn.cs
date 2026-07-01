using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class BurstShotCountColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
