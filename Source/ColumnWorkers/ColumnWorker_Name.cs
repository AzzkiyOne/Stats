using UnityEngine;
using Verse;

namespace Stats;

public class ColumnWorker_Name
    : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override Widget GetTableCellContent(string? value, ThingRec thing)
    {
        var iconStyle = new WidgetStyle()
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
            Background = (borderBox, _) =>
            {
                Widgets.DrawHighlightIfMouseover(borderBox);

                if (Widgets.ButtonInvisible(borderBox))
                {
                    Widget_DefInfoDialog.Draw(thing.Def, thing.StuffDef);
                }
            }
        };
        var icon = new Widget_Icon_Thing(thing, iconStyle);

        var labelStyle = new WidgetStyle()
        {
            TextAlign = (TextAnchor)CellStyle,
        };
        var label = new Widget_Label(value!, labelStyle);

        return new Widget_Container_Hor([icon, label], 10f, true)
        {
            Tooltip = thing.Def.description,
        };
    }
}
