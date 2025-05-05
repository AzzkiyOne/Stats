using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ThingTable.ColumnWorkers;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class CoveredBodyPartGroupsColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, HashSet<BodyPartGroupDef>> GetBodyPartGroupDefs = FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            return thing.Def.apparel?.bodyPartGroups.ToHashSet() ?? [];
        });
    private static readonly Func<ThingAlike, string> GetBodyPartGroupLabels =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            var bodyPartGroupDefs = GetBodyPartGroupDefs(thing);

            if (bodyPartGroupDefs.Count == 0)
            {
                return "";
            }

            var bodyPartGroupLabels = bodyPartGroupDefs
                .OrderBy(bodyPartGroupDef => bodyPartGroupDef.label)
                .Select(bodyPartGroupDef => bodyPartGroupDef.LabelCap);

            return string.Join("\n", bodyPartGroupLabels);
        });
    private CoveredBodyPartGroupsColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static CoveredBodyPartGroupsColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var bodyPartGroupLabels = GetBodyPartGroupLabels(thing);

        if (bodyPartGroupLabels.Length == 0)
        {
            return null;
        }

        return new Label(bodyPartGroupLabels);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget()
    {
        return new ManyToManyFilter<ThingAlike, BodyPartGroupDef>(
            GetBodyPartGroupDefs,
            DefDatabase<BodyPartGroupDef>
                .AllDefsListForReading
                .OrderBy(bodyPartGroupDef => bodyPartGroupDef.label),
            bodyPartGroupDef => new Label(bodyPartGroupDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetBodyPartGroupLabels(thing1).CompareTo(GetBodyPartGroupLabels(thing2));
    }
}
