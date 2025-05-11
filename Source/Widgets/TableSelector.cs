using System;
using System.Linq;
using Stats.Widgets.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class TableSelector : WidgetWrapper
{
    protected override Widget Widget { get; }
    public TableDef TableDef
    {
        set
        {
            IconWidget.Texture = value.Icon;
            IconColorExtension.Color = value.IconColor;
            LabelWidget.Text = value.LabelCap;
        }
    }
    private readonly FloatMenu Menu;
    private readonly Icon IconWidget;
    private readonly ColorWidgetExtension IconColorExtension;
    private readonly Label LabelWidget;
    public event Action<TableDef>? OnTableSelect;
    public TableSelector(TableDef tableDef)
    {
        Widget = new HorizontalContainer(
            [
                new Icon(tableDef.Icon, out IconWidget)
                    .PaddingAbs(Globals.GUI.PadXs)
                    .SizeAbs(MainTabWindow.TitleBarHeight)
                    .Color(tableDef.IconColor, out IconColorExtension),
                new Label(tableDef.LabelCap, out LabelWidget)
                    .HeightAbs(MainTabWindow.TitleBarHeight)
                    .TextAnchor(TextAnchor.MiddleLeft),
            ],
            Globals.GUI.Pad
        )
        .PaddingAbs(Globals.GUI.Pad, 0f)
        .Background(Verse.Widgets.LightHighlight, TexUI.HighlightTex)
        .OnClick(ShowMenu);
        Widget.Parent = this;

        var menuOptions =
            DefDatabase<TableDef>
            .AllDefs
            .Select(tableDef => new FloatMenuOption(
                tableDef.LabelCap,
                () => OnTableSelect?.Invoke(tableDef),
                tableDef.Icon,
                tableDef.IconColor
            ))
            .OrderBy(menuOption => menuOption.Label)
            .ToList();
        Menu = new(menuOptions);
    }
    private void ShowMenu()
    {
        Find.WindowStack.Add(Menu);
    }
}
