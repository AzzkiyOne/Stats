using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

static class ThingAlikes
{
    static public List<ThingAlike> list = [];
    static ThingAlikes()
    {
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
                    //var label = GenLabel.ThingLabel(thingDef, stuffDef, 0).CapitalizeFirst();
                    var label = thingDef.LabelCap + " (" + stuffDef.LabelCap + ")";
                    //var icon = thingDef.GetUIIconForStuff(stuffDef);

                    list.Add(new ThingAlike(thingDef, label, stuffDef));
                }
            }
            else
            {
                list.Add(new ThingAlike(thingDef, thingDef.LabelCap));
            }
        }
    }
}

// Implement GetHashCode (and Equals) jsut in case?
public class ThingAlike(
    ThingDef def,
    string label,
    //Texture2D icon,
    ThingDef? stuff = null
)
{
    public readonly string label = label;
    public readonly ThingDef def = def;
    public readonly ThingDef? stuff = stuff;
    //public readonly Texture2D icon = icon;
}
