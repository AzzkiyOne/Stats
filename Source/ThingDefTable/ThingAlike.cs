using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

// Implement GetHashCode (and Equals) just in case?
public class ThingAlike
{
    public required string Label { get; init; }
    public required ThingDef Def { get; init; }
    public ThingDef? Stuff { get; init; }

    private static List<ThingAlike> _all;
    public static List<ThingAlike> All
    {
        get
        {
            if (_all != null)
            {
                return _all;
            }

            _all = [];

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (
                    thingDef.IsBlueprint
                    || thingDef.IsFrame
                    || thingDef.isUnfinishedThing
                    || thingDef.IsCorpse
                    || (
                        thingDef.category != ThingCategory.Pawn
                        && thingDef.category != ThingCategory.Item
                        && thingDef.category != ThingCategory.Building
                        && thingDef.category != ThingCategory.Plant
                    )
                )
                {
                    continue;
                }

                if (thingDef.MadeFromStuff)
                {
                    var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                    foreach (var stuffDef in allowedStuffs)
                    {
                        _all.Add(new ThingAlike()
                        {
                            Label = thingDef.LabelCap + " (" + stuffDef.LabelCap + ")",
                            Def = thingDef,
                            Stuff = stuffDef,
                        });
                    }
                }
                else
                {
                    _all.Add(new ThingAlike()
                    {
                        Label = thingDef.LabelCap,
                        Def = thingDef,
                    });
                }
            }

            return _all;
        }
    }
}
