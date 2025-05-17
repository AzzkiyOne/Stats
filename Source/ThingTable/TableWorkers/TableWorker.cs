using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats.ThingTable;

public abstract class TableWorker : TableWorker<ThingAlike>
{
    private static readonly Dictionary<StuffCategoryDef, HashSet<ThingDef>> StuffsByCategory = [];
    static TableWorker()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.stuffProps == null)
            {
                continue;
            }

            foreach (var stuffCategoryDef in thingDef.stuffProps.categories)
            {
                var exists = StuffsByCategory.TryGetValue(stuffCategoryDef, out var categoryStuffs);

                if (exists)
                {
                    categoryStuffs.Add(thingDef);
                }
                else
                {
                    StuffsByCategory[stuffCategoryDef] = [thingDef];
                }
            }
        }
    }
    protected sealed override IEnumerable<ThingAlike> Records
    {
        get
        {
            // Remember that each stuff can belong to multiple categories
            // and each thing can be crafted from multiple categories of stuff.
            var allowedStuffsFor = new Dictionary<ThingDef, HashSet<ThingDef>>();

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (IsValidThingDef(thingDef) == false)
                {
                    continue;
                }

                if (thingDef.stuffCategories == null || thingDef.stuffCategories.Count == 0)
                {
                    yield return new ThingAlike(thingDef);
                }
                else
                {
                    var allowedStuffsForEntryExists = allowedStuffsFor.TryGetValue(thingDef, out var allowedStuffs);

                    if (allowedStuffsForEntryExists == false)
                    {
                        allowedStuffs = [];

                        foreach (var stuffCategoryDef in thingDef.stuffCategories)
                        {
                            allowedStuffs.AddRange(StuffsByCategory[stuffCategoryDef]);
                        }

                        allowedStuffsFor[thingDef] = allowedStuffs;
                    }

                    foreach (var stuffDef in allowedStuffsFor[thingDef])
                    {
                        yield return new ThingAlike(thingDef, stuffDef);
                    }
                }
            }
        }
    }
    protected TableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected abstract bool IsValidThingDef(ThingDef thingDef);
}
