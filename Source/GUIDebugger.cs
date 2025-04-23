using System.Collections.Generic;
using System.Diagnostics;
using Stats.Widgets;
using Stats.Widgets.Extensions;
using UnityEngine;
using Verse;

namespace Stats;

internal static class GUIDebugger
{
    private static readonly List<(string, Color, Vector2)> DebugData = new(10);
    private static readonly Color OutlineColor = Color.white.ToTransparent(0.5f);
    private const int BorderWidth = 1;
    private const float Padding = Globals.UI.Pad;
    private static Vector2 ContentSize = Vector2.zero;
    [Conditional("DEBUG")]
    public static void DebugRect(Widget widget, Rect rect)
    {
        if (!(Event.current.control && Mouse.IsOver(rect)))
        {
            return;
        }

        var name = widget.GetType().Name;
        var color = name.ToUniqueColorRGB().SaturationChanged(0.5f);
        var widthStr = rect.width.ToString("F0");
        var heightStr = rect.height.ToString("F0");
        var xStr = rect.x.ToString("F0");
        var yStr = rect.y.ToString("F0");
        var text = $"<b>{name}:</b> <i>{widthStr} x {heightStr} ({xStr}, {yStr})</i>";
        if (widget is WidgetExtension)
        {
            text = $"<b>@[</b>{text}<b>]</b>";
        }

        var textSize = Text.CalcSize(text);

        DebugData.Add((text, color, textSize));

        // Verse.Text.CalcSize does not account for bold text.
        var textWidthCalcError = name.Length * 1f;
        ContentSize.x = Mathf.Max(textSize.x + textWidthCalcError, ContentSize.x);
        ContentSize.y += textSize.y;

        Verse.Widgets.DrawBoxSolidWithOutline(rect, color, OutlineColor);
        TooltipHandler.ClearTooltipsFrom(rect);
    }
    [Conditional("DEBUG")]
    public static void DrawDebugInfo(Rect viewRect)
    {
        if (DebugData.Count == 0)
        {
            return;
        }

        DebugData.Reverse();

        ContentSize.x += (Padding + BorderWidth) * 2f;
        ContentSize.y += BorderWidth * 2f;
        var pos = GenUI.GetMouseAttachedWindowPos(ContentSize.x, ContentSize.y);
        var rect = new Rect(pos, ContentSize);

        // GenUI.GetMouseAttachedWindowPos actually does this already, but it
        // uses screen size instead of window size (which is passed as viewRect).
        if (rect.xMax > viewRect.xMax)
        {
            rect.x -= rect.width;
        }
        if (rect.yMax > viewRect.yMax)
        {
            rect.y -= rect.height;
        }

        Verse.Widgets.DrawBoxSolidWithOutline(
            rect,
            Verse.Widgets.WindowBGFillColor,
            Color.white,
            BorderWidth
        );

        rect = rect.ContractedBy(BorderWidth);

        foreach ((var text, var bgColor, var size) in DebugData)
        {
            rect.height = size.y;

            Verse.Widgets.DrawBoxSolid(rect, bgColor);

            var origGUIColor = GUI.color;
            if (bgColor.WithinDiffThresholdFrom(Color.white, 1.1f))
            {
                GUI.color = Color.black;
            }

            Verse.Widgets.Label(rect.ContractedBy(Padding, 0f), text);

            GUI.color = origGUIColor;

            rect.y += size.y;
        }

        Reset();
    }
    private static void Reset()
    {
        ContentSize = Vector2.zero;
        DebugData.Clear();
    }
}
