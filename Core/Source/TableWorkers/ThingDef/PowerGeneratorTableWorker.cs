using RimWorld;

namespace Stats.TableWorkers.ThingDef;

// TODO: Not every power generator is a building (mods).
public sealed class PowerGeneratorTableWorker :
    BuildingDefTableWorker
{
    public PowerGeneratorTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties)
    {
        CompProperties_Power? powerCompProperties = thingDef.GetCompProperties<CompProperties_Power>();

        return powerCompProperties?.PowerConsumption < 0f;
    }
}
