using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Misc;

namespace Stats.ColumnWorkers;

public class ContentSourceColumnWorker
    : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override IWidget GetTableCellContent(string? value, ThingAlike thing)
    {
        var tooltip = thing.Def.modContentPack.PackageIdPlayerFacing;
        IWidget widget = new Label(value);
        new DrawTooltipOnHover(ref widget, tooltip);

        return widget;
    }
}
