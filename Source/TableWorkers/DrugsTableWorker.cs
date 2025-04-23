using Verse;

namespace Stats.TableWorkers;

public sealed class DrugsTableWorker : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsDrug;
    }
}
