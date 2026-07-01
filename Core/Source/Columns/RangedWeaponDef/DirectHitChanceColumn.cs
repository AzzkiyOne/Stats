using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class DirectHitChanceColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;
        float directHitChance = verbProps.ForcedMissRadius > 0f
            ? 100f / GenRadial.NumCellsInRadius(verbProps.ForcedMissRadius)
            : 100f;

        return new NumberCell(directHitChance.ToDecimal(1), "0.0\\%");
    }
}
