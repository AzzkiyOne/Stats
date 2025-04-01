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
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        void onDrawIcon(ref Rect rect)
        {
            Widgets.DrawHighlightIfMouseover(rect);

            if (Widgets.ButtonInvisible(rect))
            {
                Widget_DefInfoDialog.Draw(thing.Def, thing.StuffDef);
            }
        }

        IWidget
        icon = new Widget_Icon_Thing(thing);
        icon = new WidgetComp_Size_Abs(icon, Text.LineHeight);
        icon = new WidgetComp_Generic(icon, onDrawIcon);
        IWidget
        label = new Widget_Label(value!);
        label = new WidgetComp_Width_Rel(label, 1f);
        IWidget
        container = new Widget_Container_Hor([icon, label], 10f);
        container = new WidgetComp_Tooltip(container, thing.Def.description);

        return container;
    }
}
