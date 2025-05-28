using System.Collections.Generic;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatedAtColumnWorker : ThingDefSetColumnWorker<ThingAlike, ThingDef>
{
    public CreatedAtColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(ThingAlike thing)
    {
        var thingCanBeCrafted = ThingCraftingBenches.TryGetValue(thing.Def, out var craftingBenchesDefs);

        if (thingCanBeCrafted)
        {
            return craftingBenchesDefs;
        }

        return [];
    }
    private static readonly Dictionary<ThingDef, HashSet<ThingDef>> ThingCraftingBenches = [];
    static CreatedAtColumnWorker()
    {
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefs)
        {
            // See Verse.ThingDef.SpecialDisplayStats()
            if (recipeDef is { products.Count: 1, IsSurgery: false })
            {
                var producedThingDef = recipeDef.products[0]?.thingDef;

                if (producedThingDef == null)
                {
                    continue;
                }

                if (recipeDef.recipeUsers?.Count > 0)
                {
                    var craftingBenchesEntryExists = ThingCraftingBenches.TryGetValue(producedThingDef, out var craftingBenches);

                    if (craftingBenchesEntryExists)
                    {
                        craftingBenches.AddRange(recipeDef.recipeUsers);
                    }
                    else
                    {
                        ThingCraftingBenches[producedThingDef] = [.. recipeDef.recipeUsers];
                    }
                }
            }
        }
    }
}
