using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers.Apparel;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class CoverageColumnWorker : ColumnWorker<IEnumerable<BodyPartGroupDef>>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;

    protected override IEnumerable<BodyPartGroupDef> GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.bodyPartGroups.Distinct() ?? [];
    }
    protected override Widget GetTableCellContent(IEnumerable<BodyPartGroupDef> bodyPartGroupDefs, ThingAlike thing)
    {
        var bodyPartGroupLabels = bodyPartGroupDefs
            .OrderBy(bodyPartGroupDef => bodyPartGroupDef.label)
            .Select(bodyPartGroupDef => bodyPartGroupDef.LabelCap);

        return new Label(string.Join("\n", bodyPartGroupLabels));
    }
    public override FilterWidget GetFilterWidget()
    {
        return new EnumerableFilter<BodyPartGroupDef>(
            GetValueCached,
            DefDatabase<BodyPartGroupDef>.AllDefsListForReading.OrderBy(bodyPartGroupDef => bodyPartGroupDef.label),
            bodyPartGroupDef => new Label(bodyPartGroupDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).Count().CompareTo(GetValueCached(thing2).Count());
    }
}
