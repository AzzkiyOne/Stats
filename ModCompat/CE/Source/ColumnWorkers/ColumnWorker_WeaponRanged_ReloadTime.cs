using CombatExtended;
using RimWorld;

namespace Stats.Compat.CE;

public class ColumnWorker_WeaponRanged_ReloadTime
    : ColumnWorker_Stat
{
    public override float GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == false) return 0f;

        return ColumnDef.stat!.Worker.GetValue(statReq);
    }
}
