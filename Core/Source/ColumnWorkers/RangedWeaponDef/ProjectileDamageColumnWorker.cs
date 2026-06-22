using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class ProjectileDamageColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProperties = record.PrimaryVerbProperties;
        ProjectileProperties? defaultProjProps = verbProperties.defaultProjectile?.projectile;

        if (defaultProjProps?.damageDef?.harmsHealth == true)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            decimal projectileDamage = defaultProjProps.GetDamageAmount(thingDef, null);

            return new NumberCell(projectileDamage);
        }

        return default;
    }
}
