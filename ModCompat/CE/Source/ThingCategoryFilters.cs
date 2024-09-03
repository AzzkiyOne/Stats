using Verse;

namespace Stats.Compat.CE;

public static class ThingCategoryFilters
{
    public static bool IsAmmo(ThingDef def)
    {
        return def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("Ammo"));
    }
}
