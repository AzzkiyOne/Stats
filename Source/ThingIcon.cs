using UnityEngine;
using Verse;

namespace Stats;

internal sealed class ThingIcon
{
    ThingDef ThingDef { get; }
    ThingDef? StuffDef { get; }
    public ThingIcon(ThingDef thingDef, ThingDef? stuffDef = null)
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
