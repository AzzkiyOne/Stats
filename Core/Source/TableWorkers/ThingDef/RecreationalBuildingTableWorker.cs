using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class RecreationalBuildingTableWorker :
    BuildingDefTableWorker
{
    public RecreationalBuildingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties)
    {
        float? joyGainFactor = thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f);

        return joyGainFactor > 0f;
    }
}
