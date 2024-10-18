using UnityEngine;
using Verse;

namespace Stats;

internal class WindowTitleBarWidget
{
    private readonly TableSelectorWidget TableSelector;
    public WindowTitleBarWidget(TableSelectorWidget tableSelector)
    {
        TableSelector = tableSelector;
    }
    public WindowTitleBarWidgetEvent? Draw(Rect targetRect)
    {
        var buttonWidth = targetRect.height;
        var labelWidth = targetRect.width - buttonWidth * 4;
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

        if (Widgets.ButtonImage(
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
