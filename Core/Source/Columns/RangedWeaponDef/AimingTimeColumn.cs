using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class AimingTimeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    private static readonly string FormatString = "0.00 " + "LetterSecond".Translate();

    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;
        float aimingTime = verbProps.warmupTime;

        return new NumberCell(aimingTime.ToDecimal(2), FormatString);
    }
}
