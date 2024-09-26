using UnityEngine;
using Verse;

namespace Stats;

public sealed class ThingIcon
{
    ThingDef ThingDef { get; }
    ThingDef? StuffDef { get; }
    public ThingIcon(ThingDef thingDef, ThingDef? stuffDef)
    {
        ThingDef = thingDef;
        StuffDef = stuffDef;
    }
    public void Draw(Rect targetRect)
    {
        // This is very expensive.
        Widgets.DefIcon(targetRect, ThingDef, StuffDef);
    }
}
