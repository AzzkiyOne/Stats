using System;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Misc;

// Just to make RW stop throwing warning regarding assets loading.
[StaticConstructorOnStartup]
internal sealed class WindowTitleBar
    : WidgetDecorator
{
    protected override IWidget Widget { get; }
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
    public WindowTitleBar(
        IWidget tableSelector,
        Action resetWindow,
        Action expandWindow,
        Action closeWindow,
        Action resetTableFilters
    )
    {
        new IncreaseSizeByRel(ref tableSelector, 0f, 1f, 0f, 0f);

        IWidget resetTableFiltersBtn = new Icon(TexUI.RotRightTex);
        AsIcon(ref resetTableFiltersBtn, resetTableFilters, "Reset filters");

        IWidget dragIcon = new Icon(HoldToDragTex);
        AsIcon(ref dragIcon, "Hold to drag the window (if there's nothing else to hold on to)");

        IWidget infoIcon = new Icon(TexButton.Info);
        AsIcon(ref infoIcon, Manual);

        IWidget resetWindowBtn = new Icon(ResetWindowTex);
        AsIcon(ref resetWindowBtn, resetWindow, "Reset");

        IWidget expandWindowBtn = new Icon(ExpandWindowTex);
        AsIcon(ref expandWindowBtn, expandWindow, "Expand");

        IWidget closeWindowBtn = new Icon(TexButton.CloseXSmall);
        AsIcon(ref closeWindowBtn, closeWindow, "Close", IconPadding + 2f);

        IWidget container = new HorizontalContainer(
            [
                tableSelector,
                resetTableFiltersBtn,
                dragIcon,
                infoIcon,
                resetWindowBtn,
                expandWindowBtn,
                closeWindowBtn,
            ],
            GenUI.Pad,
            true
        );
        new DrawTexture(ref container, Verse.Widgets.LightHighlight);

        Widget = container;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Verse.Widgets.DrawLineHorizontal(
            rect.x,
            rect.yMax - 1f,
            rect.width,
            MainTabWindowWidget.BorderLineColor
        );

        Widget.Draw(rect, containerSize);
    }
    private static void AsIcon(
        ref IWidget widget,
        Action clickEventHandler,
        string tooltip,
        float pad = IconPadding
    )
    {
        AsIcon(ref widget, tooltip, pad);
        new DrawTextureOnHover(ref widget, TexUI.HighlightTex);
        new AddClickEventHandler(ref widget, clickEventHandler);
    }
    private static void AsIcon(ref IWidget widget, string tooltip, float pad = IconPadding)
    {
        new IncreaseSizeByAbs(ref widget, pad);
        new SetSizeToAbs(ref widget, MainTabWindowWidget.TitleBarHeight);
        new DrawTooltipOnHover(ref widget, tooltip);
    }

    static WindowTitleBar()
    {
        HoldToDragTex = ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        ResetWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ResetWindow");
    }
}
