using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileBDFImpassableColumn<TRecord>(ColumnDef columnDef) :
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
            float projectileBDFImpassable = defaultProjDamageDef.buildingDamageFactorImpassable * 100f;

            return new NumberCell(projectileBDFImpassable, "0\\%");
        }

        return default;
    }
}
