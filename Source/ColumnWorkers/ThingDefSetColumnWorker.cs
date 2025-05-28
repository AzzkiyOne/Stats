using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefSetColumnWorker<TObject, TValue> : DefSetColumnWorker<TObject, TValue> where TValue : ThingDef
{
    protected ThingDefSetColumnWorker(ColumnDef columnDef, bool cached = true) : base(columnDef, cached)
    {
    }
    public override Widget? GetTableCellWidget(TObject @object)
    {
        var thingDefs = GetCachedValue(@object);

        if (thingDefs.Count == 0)
        {
            return null;
        }

        var icons = new List<Widget>(thingDefs.Count);

        foreach (var thingDef in thingDefs.OrderBy(thingDef => thingDef.label))
        {
            var icon = new ThingIcon(thingDef).ToButtonGhostly(() => Draw.DefInfoDialog(thingDef), thingDef.LabelCap);

            icons.Add(icon);
        }

        return new HorizontalContainer(icons, Globals.GUI.PadSm);
    }
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        return Make.MTMThingDefFilter(GetCachedValue, tableRecords);
    }
}
