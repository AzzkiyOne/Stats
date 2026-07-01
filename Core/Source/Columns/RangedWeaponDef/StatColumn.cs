using Stats.TableRecords;

namespace Stats.Columns.RangedWeaponDef;

public sealed class StatColumn<TRecord>(StatColumnDef columnDef) :
    BuildableDef.StatColumn<TRecord>(columnDef)
        where TRecord :
            IBuildableDefTableRecord,
            IRangedWeaponDefTableRecord
{
    protected override StatCell MakeCell(TRecord record)
    {
        return MakeCell(record.RangedWeaponStatRequest);
    }
}
