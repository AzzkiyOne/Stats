using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Icon_Thing
    : Widget
{
    protected override Vector2 Size { get; set; }
    private readonly Texture2D Texture;
    private readonly Color Color;
    private readonly Vector2 Proportions;
    private readonly Rect Coords;
    private readonly float Scale;
    private readonly float Angle;
    private readonly Vector2 Offset;
    public Widget_Icon_Thing(ThingRec thing)
        : this(thing.Def, thing.StuffDef)
    {
    }
    public Widget_Icon_Thing(ThingDef thingDef, ThingDef? stuffDef = null)
    {
        Texture = Widgets.GetIconFor(thingDef, stuffDef) ?? BaseContent.BadTex;
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

            if (
                thingDef.uiIconPath.NullOrEmpty()
                &&
                thingDef.graphicData.linkFlags != 0
            )
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

        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    protected override void DrawContent(Rect rect)
    {
        rect.position += Offset * rect.size;
        GUI.color = Color;
        Widgets.DrawTextureFitted(
            rect,
            Texture,
            Scale,
            Proportions,
            Coords,
            Angle
        );
        GUI.color = Color.white;
    }
}
