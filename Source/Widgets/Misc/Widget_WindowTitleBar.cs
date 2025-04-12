using System;
using UnityEngine;
using Verse;

namespace Stats;

// Just to make RW stop throwing warning regarding assets loading.
[StaticConstructorOnStartup]
internal sealed class Widget_WindowTitleBar
    : WidgetDecorator
{
    private static readonly Texture2D HoldToDragTex;
    private const string Manual =
        "- Hold [Ctrl] to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.";
    protected override IWidget Widget { get; }
    public Widget_WindowTitleBar(
        IWidget tableSelector,
        Action resetWindow,
        Action expandWindow,
        Action closeWindow,
        Action resetTableFilters
    )
    {
        const float iconHeight = StatsMainTabWindow.TitleBarHeight;
        const float iconScale = 0.9f;

        new WidgetComp_Size_Inc_Rel(ref tableSelector, 0f, 1f, 0f, 0f);

        IWidget resetTableFiltersBtn = new Widget_Icon(TexUI.RotRightTex, iconScale);
        new WidgetComp_Size_Abs(ref resetTableFiltersBtn, iconHeight);
        new WidgetComp_OnClick(ref resetTableFiltersBtn, resetTableFilters);
        new WidgetComp_Tooltip(ref resetTableFiltersBtn, "Reset filters");
        new WidgetComp_Bg_Tex_Hover(ref resetTableFiltersBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref resetTableFiltersBtn);

        IWidget dragIcon = new Widget_Icon(HoldToDragTex, iconScale);
        new WidgetComp_Size_Abs(ref dragIcon, iconHeight);
        new WidgetComp_Tooltip(ref dragIcon, "Hold to drag the window (if there's nothing more to hold on to)");

        IWidget infoIcon = new Widget_Icon(TexButton.Info, iconScale);
        new WidgetComp_Size_Abs(ref infoIcon, iconHeight);
        new WidgetComp_Tooltip(ref infoIcon, Manual);

        IWidget resetWindowBtn = new Widget_Icon(TexButton.Reveal, iconScale);// angle: 90f
        new WidgetComp_Size_Abs(ref resetWindowBtn, iconHeight);
        new WidgetComp_OnClick(ref resetWindowBtn, resetWindow);
        new WidgetComp_Tooltip(ref resetWindowBtn, "Reset");
        new WidgetComp_Bg_Tex_Hover(ref resetWindowBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref resetWindowBtn);

        IWidget expandWindowBtn = new Widget_Icon(TexButton.ShowZones, iconScale);// angle: 90f
        new WidgetComp_Size_Abs(ref expandWindowBtn, iconHeight);
        new WidgetComp_OnClick(ref expandWindowBtn, expandWindow);
        new WidgetComp_Tooltip(ref expandWindowBtn, "Expand");
        new WidgetComp_Bg_Tex_Hover(ref expandWindowBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref expandWindowBtn);

        IWidget closeWindowBtn = new Widget_Icon(TexButton.CloseXSmall, iconScale - 0.1f);
        new WidgetComp_Size_Abs(ref closeWindowBtn, iconHeight);
        new WidgetComp_OnClick(ref closeWindowBtn, closeWindow);
        new WidgetComp_Tooltip(ref closeWindowBtn, "Close");
        new WidgetComp_Bg_Tex_Hover(ref closeWindowBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref closeWindowBtn);

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

    static Widget_WindowTitleBar()
    {
        HoldToDragTex = ContentFinder<Texture2D>.Get("UI/Icons/Trainables/Tameness");
    }
}
