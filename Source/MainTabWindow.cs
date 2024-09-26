﻿using RimWorld;
using Stats.Table.Columns;
using UnityEngine;
using Verse;

namespace Stats;

public class StatsMainTabWindow : MainTabWindow
{
    protected override float Margin { get => 1f; }
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    private Rect? PreCloseRect = null;
    private Rect? PreExpandRect = null;
    private bool IsExpanded => PreExpandRect != null;
    private Rect ExpandRect => new(
        0f,
        0f,
        UI.screenWidth,
        windowRect.height = UI.screenHeight - MainButtonDef.ButtonHeight
    );
    private const float TitleBarHeight = 30f;
    //private const float tablesBrowserWidth = 300f;
    public static readonly Color BorderLineColor = new(1f, 1f, 1f, 0.4f);
    private Table.Table Table;
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;
        Table = new(ThingAlike.All, DefDatabase<Column>.AllDefsListForReading);
    }
    public override void DoWindowContents(Rect targetRect)
    {
        var titleBarText = "Things";
        var currY = targetRect.y;

        using (new TextWordWrapCtx(false))
        {
            switch (TitleBar.Draw(
                targetRect.CutFromY(ref currY, TitleBarHeight),
                titleBarText
            ))
            {
                case TitleBarEvent.Minimize:
                    Minimize();
                    break;
                case TitleBarEvent.Expand:
                    ExpandOrCollapse();
                    break;
                case TitleBarEvent.Close:
                    Close();
                    break;
            }

            DrawContent(targetRect.CutFromY(ref currY));
        }
    }
    private void DrawContent(Rect targetRect)
    {
        var currX = targetRect.x;

        Table.Draw(targetRect.CutFromX(ref currX));
    }
    private void ExpandOrCollapse()
    {
        if (IsExpanded)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }
    private void Expand()
    {
        draggable = false;
        resizeable = false;

        PreExpandRect = windowRect;
        windowRect = ExpandRect;
    }
    private void Collapse()
    {
        draggable = true;
        resizeable = true;

        if (PreExpandRect is Rect _preExpandRect)
        {
            windowRect = _preExpandRect;
            PreExpandRect = null;
        }
    }
    private void Minimize()
    {
        draggable = true;
        resizeable = true;
        PreExpandRect = null;

        SetInitialSizeAndPosition();
    }
    public override void PreOpen()
    {
        base.PreOpen();

        if (IsExpanded)
        {
            windowRect = ExpandRect;
        }
        else if (PreCloseRect is Rect _preCloseRect)
        {
            windowRect = _preCloseRect;
        }
    }
    public override void PostClose()
    {
        base.PostClose();

        PreCloseRect = windowRect;
    }
}

internal enum TitleBarEvent
{
    Minimize,
    Expand,
    Close,
}

internal static class TitleBar
{
    public static TitleBarEvent? Draw(Rect targetRect, string text)
    {
        var buttonWidth = targetRect.height;
        var labelWidth = targetRect.width - buttonWidth * 4;
        var currX = targetRect.x;
        TitleBarEvent? Event = null;

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
                Event = TitleBarEvent.Minimize;
            }

            if (GUIWidgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.ShowZones,
                "Maximize/restore window",
                90f
            ))
            {

                Event = TitleBarEvent.Expand;
            }

            if (Widgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.CloseXSmall
            ))
            {
                Event = TitleBarEvent.Close;
            }
        }

        return Event;
    }
}
