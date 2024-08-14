using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

static class FakeThings
{
    static public List<FakeThing> list = [];
    static FakeThings()
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

                    list.Add(new FakeThing(thingDef, label, stuffDef));
                }
            }
            else
            {
                list.Add(new FakeThing(thingDef, thingDef.LabelCap));
            }
        }
    }
}

public class FakeThing(
    ThingDef thingDef,
    string label,
    //Texture2D icon,
    ThingDef? stuffDef = null
)
{
    public readonly string label = label;
    public readonly ThingDef thingDef = thingDef;
    public readonly ThingDef? stuffDef = stuffDef;
    //public readonly Texture2D icon = icon;
}
