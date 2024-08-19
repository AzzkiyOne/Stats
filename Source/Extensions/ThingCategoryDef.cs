using System.Collections.Generic;
using Verse;

namespace Stats;

static class ThingCategoryDef_Extensions
{
    // What about duplicates?
    public static IEnumerable<ThingDef> AllThingDefs(this ThingCategoryDef categoryDef)
    {
        foreach (var thingDef in categoryDef.childThingDefs)
        {
            yield return thingDef;
        }

        foreach (var childCat in categoryDef.childCategories)
        {
            foreach (var thingDef in childCat.AllThingDefs())
            {
                yield return thingDef;
            }
        }
    }
}
