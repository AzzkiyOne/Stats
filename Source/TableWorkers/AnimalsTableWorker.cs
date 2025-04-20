using Verse;

namespace Stats.TableWorkers;

public class AnimalsTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.race?.Animal == true && thingDef.IsCorpse == false;
    }
}
