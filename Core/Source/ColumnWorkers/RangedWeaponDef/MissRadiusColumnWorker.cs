using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class MissRadiusColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
