using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class RangeColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;
        float range = verbProps.range;

        return new NumberCell(range);
    }
}
