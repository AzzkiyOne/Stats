using CombatExtended;
using RimWorld;

namespace Stats.Compat.CE;
public class ColumnWorker_ReloadTime : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var stat = CE_StatDefOf.ReloadTime;
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == true)
        {
            return stat.Worker.GetValue(statReq);
        }

        return 0f;
    }
}
