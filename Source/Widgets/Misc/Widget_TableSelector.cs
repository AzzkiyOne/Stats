using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_TableSelector
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
    private readonly Widget_Texture Icon;
    private readonly WidgetComp_Color IconColorComp;
    private readonly Widget_Label Label;
    protected override IWidget Widget { get; }
    public Widget_TableSelector()
        : base()
    {
        IWidget icon = Icon = new Widget_Texture(CurTableDef.Icon, 0.9f);
        new WidgetComp_Size_Abs(ref icon, StatsMainTabWindow.TitleBarHeight);
        IconColorComp =
        new WidgetComp_Color(ref icon, CurTableDef.IconColor);

        IWidget label = Label = new Widget_Label(CurTableDef.LabelCap);
        new WidgetComp_Height_Abs(ref label, StatsMainTabWindow.TitleBarHeight);
        new WidgetComp_TextAnchor(ref label, TextAnchor.MiddleLeft);

        IWidget button = new Widget_Container_Hor([icon, label], GenUI.Pad);
        new WidgetComp_Size_Inc_Abs(ref button, GenUI.Pad, 0f);
        new WidgetComp_Bg_Tex_Alt(ref button,
            Widgets.LightHighlight,
            TexUI.HighlightTex
        );
        new WidgetComp_OnClick(ref button, ShowMenu);

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
