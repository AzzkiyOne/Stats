using UnityEngine;
using Verse;

namespace Stats;

internal static class ButtonImageWidget
{
    public static bool Draw(
        Rect targetRect,
        Texture2D tex,
        string tooltip = "",
        float angle = 0f
    )
    {
        GUI.color = Mouse.IsOver(targetRect) ? GenUI.MouseoverColor : Color.white;
        Widgets.DrawTextureRotated(targetRect, tex, angle);
        GUI.color = Color.white;

        if (tooltip != "")
        {
            TooltipHandler.TipRegion(targetRect, tooltip);
        }

        return Widgets.ButtonInvisible(targetRect);
    }
}
