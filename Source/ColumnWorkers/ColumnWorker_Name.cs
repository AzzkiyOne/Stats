using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Verse;

namespace Stats;

public class ColumnWorker_Name
    : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        void handleIconClick()
        {
            Widget_DefInfoDialog.Draw(thing.Def, thing.StuffDef);
        }

        IWidget icon = new Widget_Icon_Thing(thing);
        new WidgetComp_Bg_Tex_Hover(ref icon, TexUI.HighlightTex);
        new WidgetComp_OnClick(ref icon, handleIconClick);

        IWidget label = new Widget_Label(value);

        IWidget container = new Widget_Container_Hor([icon, label], 10f);
        new WidgetComp_Tooltip(ref container, thing.Def.description);

        return container;
    }
}
