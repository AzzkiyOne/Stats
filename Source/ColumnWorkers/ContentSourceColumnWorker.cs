using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class ContentSourceColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        return ModContentPackToWidget(thing.Def.modContentPack);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new OneToManyFilter<ModContentPack>(
            thing => thing.Def.modContentPack,
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
        var modName1 = thing1.Def.modContentPack.Name;
        var modName2 = thing2.Def.modContentPack.Name;

        return modName1.CompareTo(modName2);
    }
}
