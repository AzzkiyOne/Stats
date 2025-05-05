using System;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class ContentSourceColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Func<TObject, ModContentPack> GetModContentPack;
    public ContentSourceColumnWorker(Func<TObject, ModContentPack> valueFunction) : base(TableColumnCellStyle.String)
    {
        GetModContentPack = valueFunction;
    }
    public override Widget? GetTableCellWidget(TObject @object)
    {
        return ModContentPackToWidget(GetModContentPack(@object));
    }
    public override FilterWidget<TObject> GetFilterWidget()
    {
        return new OneToManyFilter<TObject, ModContentPack>(
            thing => GetModContentPack(thing),
            LoadedModManager.RunningMods.OrderBy(mod => mod.Name),
            ModContentPackToWidget
        );
    }
    private static Widget ModContentPackToWidget(ModContentPack mod)
    {
        return new Label(mod.Name).Tooltip(mod.PackageIdPlayerFacing);
    }
    public override int Compare(TObject object1, TObject object2)
    {
        var modName1 = GetModContentPack(object1).Name;
        var modName2 = GetModContentPack(object2).Name;

        return modName1.CompareTo(modName2);
    }
}
