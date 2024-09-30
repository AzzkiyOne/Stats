using CombatExtended;
using RimWorld;

namespace Stats.Compat.CE;

public static class ThingProps
{
    public static float? ReloadTime(ThingAlike thing)
    {
        var stat = CE_StatDefOf.ReloadTime;
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        return stat.Worker.GetValue(statReq);
    }
}
