using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

public abstract class TableWorker : ITableWorker
{
    public TableDef TableDef { get; set; }
    public IEnumerable<ThingRec> GetRecords()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
        {
            if (IsValidThingDef(thingDef) == false) continue;

            if (thingDef.MadeFromStuff == false) yield return new(thingDef);

            var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

            foreach (var stuffDef in allowedStuffs)
            {
                yield return new(thingDef, stuffDef);
            }
        }
    }
    protected abstract bool IsValidThingDef(ThingDef thingDef);
}
