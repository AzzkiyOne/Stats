using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class NameColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly Func<ThingAlike, string> GetThingLabel = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.StuffDef == null
            ? thing.Def.LabelCap.RawText
            : $"{thing.StuffDef.LabelCap} {thing.Def.label}"
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing).ToButtonSubtle(openDefInfoDialog),
                new Label(GetThingLabel(thing)),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new StringFilter(GetThingLabel);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }
}
