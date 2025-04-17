using System.Linq;

namespace Stats;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public class ColumnWorker_Apparel_Coverage
    : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        var labels =
            thing
            .Def
            .apparel?
            .bodyPartGroups
            .Distinct()
            .OrderBy(def => def.label)
            .Select(part => part.LabelCap);

        return string.Join("\n", labels);
    }
}
