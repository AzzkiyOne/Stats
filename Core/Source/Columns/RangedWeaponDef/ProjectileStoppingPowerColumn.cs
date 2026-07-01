using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileStoppingPowerColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProperties = record.PrimaryVerbProperties;
        ProjectileProperties? defaultProjProps = verbProperties.defaultProjectile?.projectile;

        if (defaultProjProps != null)
        {
            float projectileStoppingPower = defaultProjProps.stoppingPower;

            return new NumberCell(projectileStoppingPower.ToDecimal(1), "0.0");
        }

        return default;
    }
}
