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
            DefInfoDialogWidget.Draw(thing.Def, thing.StuffDef);
        }

        IWidget icon = new ThingIconWidget(thing);
        new TextureHoverWidgetComp(ref icon, TexUI.HighlightTex);
        new OnClickWidgetComp(ref icon, handleIconClick);

        IWidget label = new LabelWidget(value);

        IWidget container = new HorizontalContainerWidget([icon, label], 10f);
        new TooltipWidgetComp(ref container, thing.Def.description);

        return container;
    }
}
