using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.RangedWeaponDef;

public sealed class ProjectileArmorPenetrationColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IRangedWeaponDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        VerbProperties verbProps = record.PrimaryVerbProperties;
        ProjectileProperties? defaultProjProps = verbProps.defaultProjectile?.projectile;
        const string formatString = "0\\%";

        if (defaultProjProps?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            float cellValue = defaultProjProps.GetArmorPenetration(null) * 100f;

            return new NumberCell(cellValue, formatString);
        }
        else if (defaultProjProps == null && verbProps?.beamDamageDef != null)
        {
            float cellValue = verbProps.beamDamageDef.defaultArmorPenetration * 100f;

            return new NumberCell(cellValue, formatString);
        }

        return default;
    }
}
