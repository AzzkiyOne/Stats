using Verse;

namespace Stats;

public class TableWorker_Plants
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant;
    }
}
