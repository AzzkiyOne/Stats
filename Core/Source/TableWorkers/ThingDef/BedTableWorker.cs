using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class BedTableWorker :
    BuildingDefTableWorker
{
    public BedTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties)
    {
        return thingDef.IsBed;
    }
}
