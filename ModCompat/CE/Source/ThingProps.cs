using CombatExtended;
using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public static class ThingProps
{
    public static float ReloadTime(ThingDef thingDef, ThingDef? stuffDef)
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
