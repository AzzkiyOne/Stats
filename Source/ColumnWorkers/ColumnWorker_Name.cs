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
        var labelStyle = new WidgetStyle()
        {
            TextAlign = (TextAnchor)CellStyle,
        };
        var iconStyle = new WidgetStyle()
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
        };
        void onDrawIcon(Rect rect)
        {
            Widgets.DrawHighlightIfMouseover(rect);

            if (Widgets.ButtonInvisible(rect))
            {
                Widget_DefInfoDialog.Draw(thing.Def, thing.StuffDef);
            }
        }

        return
            new Widget_Addon_Tooltip(
                new Widget_Container_Hor([
                    new Widget_Addon_Generic(
                        new Widget_Icon_Thing(thing, iconStyle),
                        onDrawIcon
                    ),
                    new Widget_Label(value!, labelStyle)
                ], 10f, true),
                thing.Def.description
            );
    }
}
