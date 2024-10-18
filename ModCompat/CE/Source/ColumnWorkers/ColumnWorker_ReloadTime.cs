using CombatExtended;
using RimWorld;
using Verse;

namespace Stats.Compat.CE;
public class ColumnWorker_ReloadTime : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var stat = CE_StatDefOf.ReloadTime;
        var statReq = StatRequest.For(thingDef, stuffDef);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == true)
        {
            return stat.Worker.GetValue(statReq);
        }

        return 0f;
    }
}
