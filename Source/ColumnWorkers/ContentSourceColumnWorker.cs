using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Extensions;
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
        return new Label(value)
            .Tooltip(thing.Def.modContentPack.PackageIdPlayerFacing);
    }
}
