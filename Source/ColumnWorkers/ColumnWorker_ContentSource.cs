using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Misc;

namespace Stats;

public class ColumnWorker_ContentSource
    : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        var tooltip = thing.Def.modContentPack.PackageIdPlayerFacing;
        IWidget widget = new Widget_Label(value);
        new WidgetComp_Tooltip(ref widget, tooltip);

        return widget;
    }
}
