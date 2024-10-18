using Verse;

namespace Stats;

public static class ThingProps
{
    public static bool IsMeleeWeapon(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
    public static bool IsRangedWeapon(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
    public static bool IsApparel(ThingDef thingDef)
    {
        return thingDef.IsApparel;
    }
}
