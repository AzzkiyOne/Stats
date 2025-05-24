using System;
using RimWorld;
using Verse;

namespace Stats.ThingTable;

public static class VerseThingDefExtensions
{
    // GenStuff.DefaultStuffFor() is a bit too heavy for some tasks.
    private static readonly Func<ThingDef, ThingDef?> GetDefaultStuffCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return GenStuff.DefaultStuffFor(thingDef);
    });
    // Verse.ThingDef.defaultStuff isn't actually what is advertised.
    public static ThingDef? GetDefaultStuff(this ThingDef thingDef)
    {
        return GetDefaultStuffCached(thingDef);
    }
    private static readonly Func<ThingDef, CompProperties_Power?> GetPowerCompPropertiesCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.GetCompProperties<CompProperties_Power>();
    });
    public static CompProperties_Power? GetPowerCompProperties(this ThingDef thingDef)
    {
        return GetPowerCompPropertiesCached(thingDef);
    }
    private static readonly Func<ThingDef, CompProperties_Refuelable?> GetRefuelableCompPropertiesCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.GetCompProperties<CompProperties_Refuelable>();
    });
    public static CompProperties_Refuelable? GetRefuelableCompProperties(this ThingDef thingDef)
    {
        return GetRefuelableCompPropertiesCached(thingDef);
    }
}
