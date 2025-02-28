using Verse;

namespace Stats;

public class TableWorker_Stuff : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        // This is to exclude chunks.
        return thingDef.stuffProps?.categories.Count > 0;
    }
}
