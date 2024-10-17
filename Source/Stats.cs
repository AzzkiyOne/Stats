using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class StatsMod
{
    static StatsMod()
    {
    }
    // TODO: Exclude CE ammo.
    public static bool IsReasonableThingDef(ThingDef thingDef)
    {
        return thingDef is
        {
            IsBlueprint: false,
            IsFrame: false,
            isUnfinishedThing: false,
            IsCorpse: false,
            category: ThingCategory.Pawn
                or ThingCategory.Item
                or ThingCategory.Building
                or ThingCategory.Plant,
        };
    }
}
