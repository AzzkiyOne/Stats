using System.Collections.Generic;
using RimWorld;
using Stats.TableWorkers;
using Verse;

namespace Stats.ThingTable.TableWorkers;

public abstract class TableWorker : TableWorker<ThingAlike>
{
    public sealed override IEnumerable<ThingAlike> GetRecords()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
        {
            if (IsValidThingDef(thingDef) == false)
            {
                continue;
            }

            if (thingDef.MadeFromStuff == false)
            {
                yield return new(thingDef);
            }

            var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

            foreach (var stuffDef in allowedStuffs)
            {
                yield return new(thingDef, stuffDef);
            }
        }
    }
    protected abstract bool IsValidThingDef(ThingDef thingDef);
}
