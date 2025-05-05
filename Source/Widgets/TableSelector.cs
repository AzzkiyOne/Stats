using System.Linq;
using Stats.Defs;
using Stats.Widgets.Extensions.Color;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class TableSelector : WidgetWrapper
{
    private TableDef _CurTableDef;
    public TableDef CurTableDef
    {
        get => _CurTableDef;
        private set
        {
            if (_CurTableDef == value)
            {
                return;
            }

            _CurTableDef = value;
            IconWidget.Texture = value.Icon;
            IconColorExtension.Color = value.IconColor;
            LabelWidget.Text = value.LabelCap;
        }
    }
    private readonly FloatMenu Menu;
    private readonly Icon IconWidget;
    private readonly ColorWidgetExtension IconColorExtension;
    private readonly Label LabelWidget;
    protected override Widget Widget { get; }
    public TableSelector()
    {
        _CurTableDef = DefDatabase<TableDef>.GetNamed("RangedWeapons_ThingTable");
        Widget = new HorizontalContainer(
            [
                new Icon(_CurTableDef.Icon, out IconWidget)
                    .PaddingAbs(Globals.GUI.PadXs)
                    .SizeAbs(MainTabWindow.TitleBarHeight)
                    .Color(_CurTableDef.IconColor, out IconColorExtension),
                new Label(_CurTableDef.LabelCap, out LabelWidget)
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
                () => CurTableDef = tableDef,
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
