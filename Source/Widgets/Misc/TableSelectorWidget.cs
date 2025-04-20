using System.Linq;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Misc;

internal sealed class TableSelectorWidget
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
    private readonly IconWidget Icon;
    private readonly ColorWidgetComp IconColorComp;
    private readonly LabelWidget Label;
    protected override IWidget Widget { get; }
    public TableSelectorWidget()
        : base()
    {
        IWidget icon = Icon = new IconWidget(CurTableDef.Icon);
        new WidgetComp_Size_Inc_Abs(ref icon, 3f);
        new WidgetComp_Size_Abs(ref icon, MainTabWindowWidget.TitleBarHeight);
        IconColorComp =
        new ColorWidgetComp(ref icon, CurTableDef.IconColor);

        IWidget label = Label = new LabelWidget(CurTableDef.LabelCap);
        new WidgetComp_Height_Abs(ref label, MainTabWindowWidget.TitleBarHeight);
        new TextAnchorWidgetComp(ref label, TextAnchor.MiddleLeft);

        IWidget button = new HorizontalContainerWidget([icon, label], GenUI.Pad);
        new WidgetComp_Size_Inc_Abs(ref button, GenUI.Pad, 0f);
        new TextureAlternatingWidgetComp(ref button,
            Verse.Widgets.LightHighlight,
            TexUI.HighlightTex
        );
        new OnClickWidgetComp(ref button, ShowMenu);

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
