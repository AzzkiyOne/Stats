using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefColumnWorker<TObject, TValue> : DefColumnWorker<TObject, TValue> where TValue : ThingDef?
{
    protected ThingDefColumnWorker(ColumnDef columnDef, bool cached = true) : base(columnDef, cached)
    {
    }
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var thingDef = GetCachedValue(@object);

        if (thingDef == null)
        {
            return null;
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thingDef).ToButtonGhostly(() => Draw.DefInfoDialog(thingDef)),
                new Label(thingDef.LabelCap),
            ],
            Globals.GUI.PadSm
        );
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        return Make.OTMThingDefFilter(GetCachedValue, tableRecords);
    }
}
