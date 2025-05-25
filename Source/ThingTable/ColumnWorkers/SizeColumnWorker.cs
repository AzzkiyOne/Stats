using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class SizeColumnWorker : ColumnWorker<ThingAlike>
{
    public SizeColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
    }
    private static readonly Func<ThingAlike, IntVec2> GetSize =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var size = thing.Def.size;

        // Because 4x5=5x4.
        return new IntVec2(Math.Max(size.x, size.z), Math.Min(size.x, size.z));
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        return new Label(GetSize(thing).ToStringCross());
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .Select(GetSize)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<IntVec2>(size, size.ToStringCross()));

        return Make.OTMFilter(GetSize, filterOptions);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.size.Area.CompareTo(thing2.Def.size.Area);
    }
}
