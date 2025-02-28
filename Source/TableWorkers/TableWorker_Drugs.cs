using Verse;

namespace Stats;

public class TableWorker_Drugs : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsDrug;
    }
}
