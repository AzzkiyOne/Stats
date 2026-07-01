using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileDamageColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
