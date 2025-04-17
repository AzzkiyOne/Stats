using System;
using UnityEngine;
using Verse;

namespace Stats;

// Just to make RW stop throwing warning regarding assets loading.
[StaticConstructorOnStartup]
internal sealed class Widget_WindowTitleBar
    : WidgetDecorator
{
    protected override IWidget Widget { get; }
    private static readonly Texture2D HoldToDragTex;
    private static readonly Texture2D ResetWindowTex;
    private static readonly Texture2D ExpandWindowTex;
    private const string Manual =
        "- Hold [Ctrl] to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.";
    private const float IconPadding = 3f;
    public Widget_WindowTitleBar(
        IWidget tableSelector,
        Action resetWindow,
        Action expandWindow,
        Action closeWindow,
        Action resetTableFilters
    )
    {
        new WidgetComp_Size_Inc_Rel(ref tableSelector, 0f, 1f, 0f, 0f);

        IWidget resetTableFiltersBtn = new Widget_Icon(TexUI.RotRightTex);
        AsIcon(ref resetTableFiltersBtn, resetTableFilters, "Reset filters");

        IWidget dragIcon = new Widget_Icon(HoldToDragTex);
        AsIcon(ref dragIcon, "Hold to drag the window (if there's nothing else to hold on to)");

        IWidget infoIcon = new Widget_Icon(TexButton.Info);
        AsIcon(ref infoIcon, Manual);

        IWidget resetWindowBtn = new Widget_Icon(ResetWindowTex);
        AsIcon(ref resetWindowBtn, resetWindow, "Reset");

        IWidget expandWindowBtn = new Widget_Icon(ExpandWindowTex);
        AsIcon(ref expandWindowBtn, expandWindow, "Expand");

        IWidget closeWindowBtn = new Widget_Icon(TexButton.CloseXSmall);
        AsIcon(ref closeWindowBtn, closeWindow, "Close", IconPadding + 2f);

        IWidget container = new Widget_Container_Hor(
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
        new WidgetComp_Bg_Tex(ref container, Widgets.LightHighlight);

        Widget = container;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Widgets.DrawLineHorizontal(
            rect.x,
            rect.yMax,
            rect.width,
            StatsMainTabWindow.BorderLineColor
        );

        Widget.Draw(rect, containerSize);
    }
    private static void AsIcon(
        ref IWidget widget,
        Action onClick,
        string tooltip,
        float pad = IconPadding
    )
    {
        AsIcon(ref widget, tooltip, pad);
        new WidgetComp_Bg_Tex_Hover(ref widget, TexUI.HighlightTex);
        new WidgetComp_OnClick(ref widget, onClick);
    }
    private static void AsIcon(ref IWidget widget, string tooltip, float pad = IconPadding)
    {
        new WidgetComp_Size_Inc_Abs(ref widget, pad);
        new WidgetComp_Size_Abs(ref widget, StatsMainTabWindow.TitleBarHeight);
        new WidgetComp_Tooltip(ref widget, tooltip);
    }

    static Widget_WindowTitleBar()
    {
        HoldToDragTex = ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        ResetWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ResetWindow");
    }
}
