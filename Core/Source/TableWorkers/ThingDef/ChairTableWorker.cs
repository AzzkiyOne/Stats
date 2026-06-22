using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class ChairTableWorker :
    BuildingDefTableWorker
{
    public ChairTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties)
    {
        return buildingProperties.isSittable;
    }
}
