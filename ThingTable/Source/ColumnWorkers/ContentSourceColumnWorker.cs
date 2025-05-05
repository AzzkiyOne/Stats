using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class ContentSourceColumnWorker : ColumnWorker<ThingAlike>
{
    private ContentSourceColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static ContentSourceColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        return ModContentPackToWidget(thing.Def.modContentPack);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget()
    {
        return new OneToManyFilter<ThingAlike, ModContentPack>(
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
