using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.ThingTable;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class CoveredBodyPartGroupsColumnWorker : DefSetColumnWorker<ThingAlike, BodyPartGroupDef>
{
    public CoveredBodyPartGroupsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<BodyPartGroupDef> GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.bodyPartGroups.ToHashSet() ?? [];
    }
}
