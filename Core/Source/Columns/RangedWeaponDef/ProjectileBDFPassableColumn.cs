using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileBDFPassableColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProperties = record.PrimaryVerbProperties;
        DamageDef? defaultProjDamageDef = verbProperties.defaultProjectile?.projectile?.damageDef;

        if (defaultProjDamageDef != null)
        {
            float projectileBDFPassable = defaultProjDamageDef.buildingDamageFactorPassable * 100f;

            return new NumberCell(projectileBDFPassable, "0\\%");
        }

        return default;
    }
}
