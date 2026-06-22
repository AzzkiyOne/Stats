using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class ProjectileStoppingPowerColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
