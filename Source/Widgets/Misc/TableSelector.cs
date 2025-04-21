using System.Linq;
using Stats.Widgets.Containers;
using Stats.Widgets.Extensions;
using Stats.Widgets.Extensions.Size;
using Stats.Widgets.Extensions.Size.Constraints;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Misc;

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
    private readonly SetColor IconColorExtension;
    private readonly Label LabelWidget;
    public override IWidget Widget { get; }
    public TableSelector()
        : base()
    {
        var icon = new Icon(CurTableDef.Icon)
            .PadAbs(3f)
            .SizeAbs(MainTabWindowWidget.TitleBarHeight)
            .Color(CurTableDef.IconColor);
        IconWidget = icon.Get<Icon>();
        IconColorExtension = icon.Get<SetColor>();

        var label = new Label(CurTableDef.LabelCap)
            .HeightAbs(MainTabWindowWidget.TitleBarHeight)
            .TextAnchor(TextAnchor.MiddleLeft);
        LabelWidget = label.Get<Label>();

        Widget = new HorizontalContainer([icon, label], GenUI.Pad)
            .PadAbs(GenUI.Pad, 0f)
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
