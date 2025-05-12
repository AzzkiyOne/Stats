using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats.ThingTable;

public abstract class TableWorker : TableWorker<ThingAlike>
{
    protected sealed override IEnumerable<ThingAlike> Records
    {
        get
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
    }
    protected TableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected abstract bool IsValidThingDef(ThingDef thingDef);
}
