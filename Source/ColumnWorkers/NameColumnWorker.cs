using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Verse;

namespace Stats.ColumnWorkers;

public class NameColumnWorker
    : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override IWidget GetTableCellContent(string? value, ThingAlike thing)
    {
        void handleIconClick()
        {
            DefInfoDialog.Draw(thing.Def, thing.StuffDef);
        }

        IWidget icon = new ThingIcon(thing);
        new DrawTextureOnHover(ref icon, TexUI.HighlightTex);
        new AddClickEventHandler(ref icon, handleIconClick);

        IWidget label = new Label(value);

        IWidget container = new HorizontalContainer([icon, label], 10f);
        new DrawTooltipOnHover(ref container, thing.Def.description);

        return container;
    }
}
