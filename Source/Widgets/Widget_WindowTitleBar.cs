using UnityEngine;
using Verse;

namespace Stats;

// Just to make RW stop throwing warning regarding assets loading.
[StaticConstructorOnStartup]
internal class Widget_WindowTitleBar
{
    private readonly Widget_TableSelector TableSelector;
    private static readonly Texture2D HoldToDragTex;
    private const string Manual = "- Click on the title bar to select a table.\n- Click on any row to select it. You can select multiple rows.\n- Press \"Alt\" to compare selected rows.\n- Hold \"Ctrl\" to scroll horizontally.";
    public Widget_WindowTitleBar(Widget_TableSelector tableSelector)
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
            tooltip: Manual
        );

        if (Widget_ButtonImage.Draw(
            targetRect.CutByX(buttonWidth),
            TexButton.Reveal,
            angle: 90f
        ))
        {
            Event = WindowTitleBarWidgetEvent.Minimize;
        }

        if (Widget_ButtonImage.Draw(
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
    static Widget_WindowTitleBar()
    {
        HoldToDragTex = ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
    }
}

internal enum WindowTitleBarWidgetEvent
{
    Minimize,
    Expand,
    Close,
}
