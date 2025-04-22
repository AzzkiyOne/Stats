using Verse;

namespace Stats.TableWorkers;

public sealed class PlantsTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant;
    }
}
