using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers.Generic;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class ReloadTimeColumnWorker
    : StatColumnWorker
{
    protected override float GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == false)
        {
            return 0f;
        }

        return ColumnDef.stat!.Worker.GetValue(statReq);
    }
}
