using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ContentSourceColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Func<TObject, ModContentPack?> GetCachedModContentPack;
    protected ContentSourceColumnWorker(ColumnDef columndef, bool cached = true) : base(columndef, ColumnCellStyle.String)
    {
        GetCachedModContentPack = GetModContentPack;

        if (cached)
        {
            GetCachedModContentPack = GetCachedModContentPack.Memoized();
        }
    }
    protected abstract ModContentPack? GetModContentPack(TObject @object);
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var mod = GetModContentPack(@object);

        if (mod == null)
        {
            return null;
        }

        return new Label(mod.Name).Tooltip(mod.PackageIdPlayerFacing);
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        var filterOptions = tableRecords
            .Select(GetCachedModContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );

        return Make.OTMFilter(GetCachedModContentPack, filterOptions);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var modName1 = GetCachedModContentPack(object1)?.Name;
        var modName2 = GetCachedModContentPack(object2)?.Name;

        return Comparer.Default.Compare(modName1, modName2);
    }
}
