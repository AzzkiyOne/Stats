﻿using UnityEngine;
using Verse;

namespace Stats;

internal static class WindowTitleBarWidget
{
    public static WindowTitleBarWidgetEvent? Draw(Rect targetRect, string text)
    {
        var buttonWidth = targetRect.height;
        var labelWidth = targetRect.width - buttonWidth * 4;
        var currX = targetRect.x;
        WindowTitleBarWidgetEvent? Event = null;

        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            Widgets.DrawLightHighlight(targetRect);

            Widgets.DrawLineHorizontal(
                targetRect.x,
                targetRect.yMax,
                targetRect.width,
                StatsMainTabWindow.BorderLineColor
            );

            Widgets.Label(
                targetRect
                    .CutFromX(ref currX, labelWidth)
                    .ContractedBy(GenUI.Pad, 0f),
                text
            );

            Widgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.Info,
                tooltip: "How to use:"
            );

            if (GUIWidgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.Reveal,
                angle: 90f
            ))
            {
                Event = WindowTitleBarWidgetEvent.Minimize;
            }

            if (GUIWidgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.ShowZones,
                "Maximize/restore window",
                90f
            ))
            {

                Event = WindowTitleBarWidgetEvent.Expand;
            }

            if (Widgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.CloseXSmall
            ))
            {
                Event = WindowTitleBarWidgetEvent.Close;
            }
        }

        return Event;
    }
}

internal enum WindowTitleBarWidgetEvent
{
    Minimize,
    Expand,
    Close,
}
