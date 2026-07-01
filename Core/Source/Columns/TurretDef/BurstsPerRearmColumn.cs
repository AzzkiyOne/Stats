using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.TurretDef;

public sealed class BurstsPerRearmColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
        where TRecord :
            ITurretDefTableRecord,
            IRefuelableDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps is { fuelCapacity: > 0f })
        {
            VerbProperties turretGunDefPrimaryVerbProps = record.PrimaryVerbProperties;
            float fuelPerBurst = turretGunDefPrimaryVerbProps.consumeFuelPerBurst;
            float fuelPerShot = turretGunDefPrimaryVerbProps.consumeFuelPerShot;

            if (fuelPerShot > 0f)
            {
                fuelPerBurst = fuelPerShot * turretGunDefPrimaryVerbProps.burstShotCount;
            }

            if (fuelPerBurst > 0f)
            {
                float fuelCapacity = refuelableCompProps.fuelCapacity;
                float burstsPerRearm = fuelCapacity / fuelPerBurst;

                return new NumberCell(burstsPerRearm);
            }
        }

        return default;
    }
}
