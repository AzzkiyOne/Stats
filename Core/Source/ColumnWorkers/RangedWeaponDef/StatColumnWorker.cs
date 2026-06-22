using Stats.TableRecords;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class StatColumnWorker<TRecord>(StatColumnDef columnDef) :
    BuildableDef.StatColumnWorker<TRecord>(columnDef)
        where TRecord :
            IBuildableDefTableRecord,
            IRangedWeaponDefTableRecord
{
    protected override StatCell MakeCell(TRecord record)
    {
        return MakeCell(record.RangedWeaponStatRequest);
    }
}
