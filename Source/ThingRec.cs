using Verse;

namespace Stats;

//public readonly record struct ThingRec(ThingDef Def, ThingDef? StuffDef = null);
public record class ThingRec(ThingDef Def, ThingDef? StuffDef = null);
