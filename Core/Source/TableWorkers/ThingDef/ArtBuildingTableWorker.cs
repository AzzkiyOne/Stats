using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class ArtBuildingTableWorker :
    BuildingDefTableWorker
{
    public ArtBuildingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties)
    {
        return thingDef.IsArt;
    }
}
