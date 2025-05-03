using Verse;

namespace Stats.TableWorkers;

public sealed class PlantsTableWorker : TableWorker
{
    public static PlantsTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant;
    }
}
