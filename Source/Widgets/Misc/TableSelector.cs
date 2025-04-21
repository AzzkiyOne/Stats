using System.Linq;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
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
            Icon.Tex = value.Icon;
            IconColorComp.Color = value.IconColor;
            Label.Text = value.LabelCap;
        }
    }
    private readonly FloatMenu Menu;
    private readonly Icon Icon;
    private readonly SetColor IconColorComp;
    private readonly Label Label;
    protected override IWidget Widget { get; }
    public TableSelector()
        : base()
    {
        IWidget icon = Icon = new Icon(CurTableDef.Icon);
        new IncreaseSizeByAbs(ref icon, 3f);
        new SetSizeToAbs(ref icon, MainTabWindowWidget.TitleBarHeight);
        IconColorComp =
        new SetColor(ref icon, CurTableDef.IconColor);

        IWidget label = Label = new Label(CurTableDef.LabelCap);
        new SetHeightToAbs(ref label, MainTabWindowWidget.TitleBarHeight);
        new SetTextAnchor(ref label, TextAnchor.MiddleLeft);

        IWidget button = new HorizontalContainer([icon, label], GenUI.Pad);
        new IncreaseSizeByAbs(ref button, GenUI.Pad, 0f);
        new ChangeTextureOnHover(ref button,
            Verse.Widgets.LightHighlight,
            TexUI.HighlightTex
        );
        new AddClickEventHandler(ref button, ShowMenu);

        Widget = button;

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
