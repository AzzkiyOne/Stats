using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

[StaticConstructorOnStartup]
internal sealed class MainTabWindowTitleBar : WidgetWrapper
{
    protected override Widget Widget { get; }
    private static readonly Texture2D ExpandWindowTex;
    private const string Manual =
        "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
        "  - You can pin multiple rows.\n" +
        "  - Pinned rows are unaffected by filters.";
    private const float IconPadding = Globals.GUI.PadXs;
    private readonly Label LabelWidget;
    private ITableWidget _TableWidget;
    public ITableWidget TableWidget
    {
        set
        {
            if (value == _TableWidget)
            {
                return;
            }

            _TableWidget.OnFilterModeChange -= HandleTableFilterModeChange;
            _TableWidget = value;
            value.OnFilterModeChange += HandleTableFilterModeChange;
            LabelWidget.Text = value.FilterMode.ToString();
        }
    }
    public MainTabWindowTitleBar(
        ITableWidget tableWidget,
        Widget tableSelector,
        Action expandOrResetWindow,
        Action resetTableFilters
    )
    {
        _TableWidget = tableWidget;
        tableWidget.OnFilterModeChange += HandleTableFilterModeChange;
        Widget = new HorizontalContainer(
            [
                new HorizontalContainer(
                    [
                        tableSelector,
                        ToToolbarIcon(
                            new Icon(TexUI.RotRightTex),
                            resetTableFilters,
                            "Reset filters"
                    ),
                    new Label(tableWidget.FilterMode.ToString(), out LabelWidget)
                        .HeightAbs(MainTabWindow.TitleBarHeight)
                        .PaddingAbs(Globals.GUI.PadSm, 0f)
                        .TextAnchor(TextAnchor.MiddleCenter)
                        .ToButtonGhostly(
                            () => _TableWidget.ToggleFilterMode(),
                            "Click to switch between filtering modes.\n" +
                            "This setting is individual for each table."
                        )
                    ],
                    Globals.GUI.Pad,
                    true
                ).WidthRel(1f),
                ToToolbarIcon(
                    new Icon(TexButton.Info),
                    Manual
                ),
                ToToolbarIcon(
                    new Icon(ExpandWindowTex),
                    expandOrResetWindow,
                    "Expand/Reset"
                ),
            ],
            Globals.GUI.Pad,
            true
        )
        .BorderBottom(1f, MainTabWindow.BorderLineColor)
        .Background(Verse.Widgets.LightHighlight);
        Widget.Parent = this;
    }
    private static Widget ToToolbarIcon(
        Widget widget,
        Action clickEventHandler,
        string tooltip,
        float pad = IconPadding
    )
    {
        return ToToolbarIcon(widget, tooltip, pad)
            .ToButtonGhostly(clickEventHandler);
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
    private void HandleTableFilterModeChange(TableFilterMode filterMode)
    {
        LabelWidget.Text = filterMode.ToString();
    }

    static MainTabWindowTitleBar()
    {
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
    }
}
