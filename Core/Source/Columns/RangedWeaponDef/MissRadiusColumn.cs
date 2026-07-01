using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class MissRadiusColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;
        float missRadius = verbProps.ForcedMissRadius;

        return new NumberCell(missRadius.ToDecimal(1), "0.0");
    }
}
