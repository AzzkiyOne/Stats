using Verse;

namespace Stats.TableWorkers;

public class DrugsTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsDrug;
    }
}
