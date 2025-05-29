using Verse;

namespace Stats.ThingTable;

public sealed class PlantsTableWorker : TableWorker
{
    public PlantsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
