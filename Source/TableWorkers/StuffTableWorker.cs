using Verse;

namespace Stats.TableWorkers;

public sealed class StuffTableWorker : TableWorker
{
    public static StuffTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        // This is to exclude chunks.
        return thingDef.stuffProps?.categories.Count > 0;
    }
}
