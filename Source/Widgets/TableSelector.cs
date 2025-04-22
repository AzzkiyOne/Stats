using System.Linq;
using Stats.Widgets.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class TableSelector
    : WidgetDecorator
{
    private TableDef _CurTableDef = TableDefOf.RangedWeapons;
    public TableDef CurTableDef
    {
        get => _CurTableDef;
        private set
        {
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
    public override Widget Widget { get; }
    public TableSelector()
    {
        Widget = new HorizontalContainer(
            [
                new Icon(CurTableDef.Icon, out IconWidget)
                    .PaddingAbs(3f)
                    .SizeAbs(MainTabWindow.TitleBarHeight)
                    .Color(CurTableDef.IconColor, out IconColorExtension),
                new Label(CurTableDef.LabelCap, out LabelWidget)
                    .HeightAbs(MainTabWindow.TitleBarHeight)
                    .TextAnchor(TextAnchor.MiddleLeft),
            ],
            GenUI.Pad
        )
        .PaddingAbs(GenUI.Pad, 0f)
        .Background(Verse.Widgets.LightHighlight, TexUI.HighlightTex)
        .OnClick(ShowMenu);

        var menuOptions =
            DefDatabase<TableDef>
            .AllDefs
            .Select(tableDef => new FloatMenuOption(
                tableDef.LabelCap,
                () => CurTableDef = tableDef,
                tableDef.Icon,
                tableDef.IconColor
            ))
            .OrderBy(opt => opt.Label)
            .ToList();
        Menu = new(menuOptions);
    }
    private void ShowMenu()
    {
        Find.WindowStack.Add(Menu);
    }
}
