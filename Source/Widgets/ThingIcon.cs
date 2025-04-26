using RimWorld;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class ThingIcon : Widget
{
    private readonly Texture2D Texture;
    private readonly Color Color;
    private readonly Vector2 Proportions;
    private readonly Rect Coords;
    private readonly float Scale;
    private readonly float Angle;
    private readonly Vector2 Offset;
    public ThingIcon(ThingAlike thing) : this(thing.Def, thing.StuffDef)
    {
    }
    public ThingIcon(ThingDef thingDef, ThingDef? stuffDef = null)
    {
        Texture = Verse.Widgets.GetIconFor(thingDef, stuffDef) ?? BaseContent.BadTex;
        Scale = GenUI.IconDrawScale(thingDef);
        Angle = thingDef.uiIconAngle;
        Offset = thingDef.uiIconOffset;

        if (stuffDef != null)
        {
            Color = thingDef.GetColorForStuff(stuffDef);
        }
        else
        {
            Color = thingDef.MadeFromStuff
                ? thingDef.GetColorForStuff(GenStuff.DefaultStuffFor(thingDef))
                : thingDef.uiIconColor;
        }

        if (thingDef.graphicData != null)
        {
            Proportions = thingDef.graphicData.drawSize.RotatedBy(thingDef.defaultPlacingRot);

            if (thingDef.uiIconPath.NullOrEmpty() && thingDef.graphicData.linkFlags != 0)
            {
                Coords = new Rect(0f, 0.5f, 0.25f, 0.25f);// Verse.Widgets.LinkedTexCoords
            }
            else
            {
                Coords = new Rect(0f, 0f, 1f, 1f);// Verse.Widgets.DefaultTexCoords
            }
        }
        else
        {
            Proportions = new Vector2(Texture.width, Texture.height);
        }

        Resize();
    }
    protected override Vector2 CalcSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        rect.position += Offset * rect.size;

        var origGUIColor = GUI.color;
        GUI.color = Color.AdjustedForGUIOpacity();

        Verse.Widgets.DrawTextureFitted(rect, Texture, Scale, Proportions, Coords, Angle);

        GUI.color = origGUIColor;
    }
}
