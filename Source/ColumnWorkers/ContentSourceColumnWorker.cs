using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class ContentSourceColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Func<TObject, ModContentPack?> GetModContentPack;
    public ContentSourceColumnWorker(Func<TObject, ModContentPack?> valueFunction) : base(ColumnCellStyle.String)
    {
        GetModContentPack = valueFunction;
    }
    public override Widget? GetTableCellWidget(TObject @object)
    {
        var mod = GetModContentPack(@object);

        if (mod == null)
        {
            return null;
        }

        return ModContentPackToWidget(mod);
    }
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        var modContentPacks = tableRecords
            .Select(GetModContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name ?? "");

        return new OneToManyFilter<TObject, ModContentPack?>(
            GetModContentPack,
            modContentPacks,
            mod => mod == null ? new Label("") : ModContentPackToWidget(mod)
        );
    }
    private static Widget ModContentPackToWidget(ModContentPack mod)
    {
        return new Label(mod.Name).Tooltip(mod.PackageIdPlayerFacing);
    }
    public override int Compare(TObject object1, TObject object2)
    {
        var modName1 = GetModContentPack(object1)?.Name;
        var modName2 = GetModContentPack(object2)?.Name;

        return Comparer.Default.Compare(modName1, modName2);
    }
}
