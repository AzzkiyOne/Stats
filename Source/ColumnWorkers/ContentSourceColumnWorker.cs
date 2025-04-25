using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class ContentSourceColumnWorker : ColumnWorker<ModContentPack>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    protected override ModContentPack GetValue(ThingAlike thing)
    {
        return thing.Def.modContentPack;
    }
    protected override Widget GetTableCellContent(ModContentPack mod, ThingAlike thing)
    {
        return ModContentPackToWidget(mod);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new EnumerableFilter<ModContentPack>(
            GetValueCached,
            LoadedModManager.RunningMods.OrderBy(mod => mod.Name),
            ModContentPackToWidget
        );
    }
    private static Widget ModContentPackToWidget(ModContentPack mod)
    {
        return new Label(mod.Name).Tooltip(mod.PackageIdPlayerFacing);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).Name.CompareTo(GetValueCached(thing2).Name);
    }
}
