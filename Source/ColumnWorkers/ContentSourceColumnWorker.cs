using Stats.ColumnWorkers.Generic;
using Stats.Widgets;

namespace Stats.ColumnWorkers;

public sealed class ContentSourceColumnWorker
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
