using System;
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
public sealed class CoverageColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly Func<ThingAlike, IEnumerable<BodyPartGroupDef>> GetBodyPartGroups = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.Def.apparel?.bodyPartGroups.Distinct() ?? []
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var bodyPartGroupDefs = GetBodyPartGroups(thing);

        if (bodyPartGroupDefs.Count() == 0)
        {
            return null;
        }

        var bodyPartGroupLabels = bodyPartGroupDefs
            .OrderBy(bodyPartGroupDef => bodyPartGroupDef.label)
            .Select(bodyPartGroupDef => bodyPartGroupDef.LabelCap);

        return new Label(string.Join("\n", bodyPartGroupLabels));
    }
    public override FilterWidget GetFilterWidget()
    {
        return new ManyToManyOptionsFilter<BodyPartGroupDef>(
            GetBodyPartGroups,
            DefDatabase<BodyPartGroupDef>.AllDefsListForReading.OrderBy(bodyPartGroupDef => bodyPartGroupDef.label),
            bodyPartGroupDef => new Label(bodyPartGroupDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var bodyPartGroupsCount1 = GetBodyPartGroups(thing1).Count();
        var bodyPartGroupsCount2 = GetBodyPartGroups(thing2).Count();

        return bodyPartGroupsCount1.CompareTo(bodyPartGroupsCount2);
    }
}
