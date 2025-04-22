using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Just to make RW stop throwing warning regarding assets loading.
[StaticConstructorOnStartup]
internal sealed class MainTabWindowTitleBar
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private static readonly Texture2D HoldToDragTex;
    private static readonly Texture2D ResetWindowTex;
    private static readonly Texture2D ExpandWindowTex;
    private const string Manual =
        "- Hold [Ctrl] to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Click on a row to select/deselect it.\n" +
        "  - You can select multiple rows.\n" +
        "  - Selected rows are unaffected by filters.";
    private const float IconPadding = 3f;
    public MainTabWindowTitleBar(
        Widget tableSelector,
        Action resetWindow,
        Action expandWindow,
        Action closeWindow,
        Action resetTableFilters
    )
    {
        Widget = new HorizontalContainer(
            [
                tableSelector.PaddingRel(0f, 1f, 0f, 0f),
                ToToolbarIcon(
                    new Icon(TexUI.RotRightTex),
                    resetTableFilters,
                    "Reset filters"
                ),
                ToToolbarIcon(
                    new Icon(HoldToDragTex),
                    "Hold to drag the window (if there's nothing else to hold on to)"
                ),
                ToToolbarIcon(
                    new Icon(TexButton.Info),
                    Manual
                ),
                ToToolbarIcon(
                    new Icon(ResetWindowTex),
                    resetWindow,
                    "Reset"
                ),
                ToToolbarIcon(
                    new Icon(ExpandWindowTex),
                    expandWindow,
                    "Expand"
                ),
                ToToolbarIcon(
                    new Icon(TexButton.CloseXSmall),
                    closeWindow,
                    "Close",
                    IconPadding + 2f
                ),
            ],
            GenUI.Pad,
            true
        ).Background(Verse.Widgets.LightHighlight);
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Verse.Widgets.DrawLineHorizontal(
            rect.x,
            rect.yMax - 1f,
            rect.width,
            MainTabWindow.BorderLineColor
        );

        Widget.Draw(rect, containerSize);
    }
    private static Widget ToToolbarIcon(
        Widget widget,
        Action clickEventHandler,
        string tooltip,
        float pad = IconPadding
    )
    {
        return ToToolbarIcon(widget, tooltip, pad)
            .ToButtonSubtle(clickEventHandler);
    }
    private static Widget ToToolbarIcon(
        Widget widget,
        string tooltip,
        float pad = IconPadding
    )
    {
        return widget
            .PaddingAbs(pad)
            .SizeAbs(MainTabWindow.TitleBarHeight)
            .Tooltip(tooltip);
    }

    static MainTabWindowTitleBar()
    {
        HoldToDragTex = ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        ResetWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ResetWindow");
    }
}
