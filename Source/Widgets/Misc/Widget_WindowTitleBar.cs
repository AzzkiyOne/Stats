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
        "- Hold \"Ctrl\" to scroll horizontally.\n" +
        "- Hold \"Ctrl\" and click on a column's name to pin/unpin it.";
    protected override IWidget Widget { get; }
    public Widget_WindowTitleBar(
        IWidget tableSelector,
        Action onReset,
        Action onExpand,
        Action onClose
    )
    {
        const float iconHeight = StatsMainTabWindow.TitleBarHeight;
        const float iconScale = 0.9f;

        new WidgetComp_Size_Inc_Rel(ref tableSelector, 0f, 1f, 0f, 0f);

        IWidget dragIcon = new Widget_Texture(HoldToDragTex, iconScale);
        new WidgetComp_Size_Abs(ref dragIcon, iconHeight);
        new WidgetComp_Tooltip(ref dragIcon, "Hold to drag the window (if there's nothing more to hold on to)");

        IWidget infoIcon = new Widget_Texture(TexButton.Info, iconScale);
        new WidgetComp_Size_Abs(ref infoIcon, iconHeight);
        new WidgetComp_Tooltip(ref infoIcon, Manual);

        IWidget resetBtn = new Widget_Texture(TexButton.Reveal, iconScale);// angle: 90f
        new WidgetComp_Size_Abs(ref resetBtn, iconHeight);
        new WidgetComp_OnClick(ref resetBtn, onReset);
        new WidgetComp_Tooltip(ref resetBtn, "Reset window");
        new WidgetComp_Bg_Tex_Hover(ref resetBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref resetBtn);

        IWidget expandBtn = new Widget_Texture(TexButton.ShowZones, iconScale);// angle: 90f
        new WidgetComp_Size_Abs(ref expandBtn, iconHeight);
        new WidgetComp_OnClick(ref expandBtn, onExpand);
        new WidgetComp_Tooltip(ref expandBtn, "Expand window");
        new WidgetComp_Bg_Tex_Hover(ref expandBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref expandBtn);

        IWidget closeBtn = new Widget_Texture(TexButton.CloseXSmall, iconScale - 0.1f);
        new WidgetComp_Size_Abs(ref closeBtn, iconHeight);
        new WidgetComp_OnClick(ref closeBtn, onClose);
        new WidgetComp_Tooltip(ref closeBtn, "Close");
        new WidgetComp_Bg_Tex_Hover(ref closeBtn, Widgets.LightHighlight);
        new WidgetComp_Color_Hover(ref closeBtn);

        IWidget container = new Widget_Container_Hor(
            [
                tableSelector,
                dragIcon,
                infoIcon,
                resetBtn,
                expandBtn,
                closeBtn,
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
