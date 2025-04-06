using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_TableSelector
    : Widget
{
    private TableDef _CurTableDef = TableDefOf.RangedWeapons;
    public TableDef CurTableDef
    {
        get => _CurTableDef;
        private set
        {
            _CurTableDef = value;
            Icon.Tex = value.Icon;
            IconColor.Color = value.IconColor;
            Label.Text = value.LabelCap;
        }
    }
    private readonly FloatMenu Menu;
    private readonly Widget_Texture Icon;
    private readonly WidgetComp_Color IconColor;
    private readonly Widget_Label Label;
    private readonly IWidget Button;
    public Widget_TableSelector()
        : base()
    {
        IWidget
        icon =
        Icon = new Widget_Texture(CurTableDef.Icon);
        icon = new WidgetComp_Width_Abs(icon, Text.LineHeight);
        icon = new WidgetComp_Height_Rel(icon, 1f);
        icon =
        IconColor = new WidgetComp_Color(icon, CurTableDef.IconColor);
        IWidget
        label =
        Label = new Widget_Label(CurTableDef.LabelCap);
        label = new WidgetComp_Height_Rel(label, 1f);
        label = new WidgetComp_TextAnchor(label, TextAnchor.MiddleLeft);

        Button = new Widget_Container_Hor([icon, label], GenUI.Pad);
        Button = new WidgetComp_Size_Inc_Abs(Button, GenUI.Pad, 0f);
        Button = new WidgetComp_Height_Rel(Button, 1f);
        Button = new WidgetComp_Bg_Tex_Alt(Button,
            Widgets.LightHighlight,
            TexUI.HighlightTex
        );
        Button = new WidgetComp_OnClick(Button, ShowMenu);
        Button.Parent = this;

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

        UpdateSize();
    }
    protected override Vector2 GetSize()
    {
        return Button.GetSize(Vector2.positiveInfinity);
    }
    protected override void DrawContent(Rect rect)
    {
        Button.DrawIn(rect);
    }
    private void ShowMenu()
    {
        Find.WindowStack.Add(Menu);
    }
}
