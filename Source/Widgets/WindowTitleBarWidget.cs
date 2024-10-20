using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

internal class WindowTitleBarWidget
{
    private readonly TableSelectorWidget TableSelector;
    private static readonly Texture2D? _holdToDragTex;
    private static Texture2D HoldToDragTex => _holdToDragTex ?? ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
    public WindowTitleBarWidget(TableSelectorWidget tableSelector)
    {
        TableSelector = tableSelector;
    }
    public WindowTitleBarWidgetEvent? Draw(Rect targetRect)
    {
        var buttonWidth = targetRect.height;
        var labelWidth = targetRect.width - buttonWidth * 5;
        WindowTitleBarWidgetEvent? Event = null;

        Widgets.DrawLightHighlight(targetRect);
        Widgets.DrawLineHorizontal(
            targetRect.x,
            targetRect.yMax,
            targetRect.width,
            StatsMainTabWindow.BorderLineColor
        );
        Text.Anchor = TextAnchor.MiddleLeft;
        TableSelector.Draw(targetRect.CutByX(labelWidth));
        Text.Anchor = Constants.DefaultTextAnchor;
        var rect = targetRect.CutByX(buttonWidth);
        Widgets.DrawTextureFitted(rect, HoldToDragTex, 1f);
        TooltipHandler.TipRegion(rect, "Hold to drag the window.");
        Widgets.ButtonImage(
            targetRect.CutByX(buttonWidth),
            TexButton.Info,
            tooltip: "How to use:"
        );

        if (ButtonImageWidget.Draw(
            targetRect.CutByX(buttonWidth),
            TexButton.Reveal,
            angle: 90f
        ))
        {
            Event = WindowTitleBarWidgetEvent.Minimize;
        }

        if (ButtonImageWidget.Draw(
            targetRect.CutByX(buttonWidth),
            TexButton.ShowZones,
            "Maximize/restore window",
            90f
        ))
        {

            Event = WindowTitleBarWidgetEvent.Expand;
        }

        if (Widgets.ButtonImageFitted(
            targetRect.CutByX(buttonWidth),
            TexButton.CloseXSmall
        ))
        {
            Event = WindowTitleBarWidgetEvent.Close;
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
