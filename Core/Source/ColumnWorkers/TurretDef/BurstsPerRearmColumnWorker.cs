using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Verse;

namespace Stats.ColumnWorkers.TurretDef;

public sealed class BurstsPerRearmColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
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
