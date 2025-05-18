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
}
