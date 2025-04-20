using Verse;

namespace Stats.TableWorkers;

public class StuffTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        // This is to exclude chunks.
        return thingDef.stuffProps?.categories.Count > 0;
    }
}
