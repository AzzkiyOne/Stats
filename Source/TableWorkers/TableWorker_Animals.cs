using Verse;

namespace Stats;

public class TableWorker_Animals
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.race?.Animal == true && thingDef.IsCorpse == false;
    }
}
