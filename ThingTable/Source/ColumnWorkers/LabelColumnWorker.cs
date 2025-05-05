using System;
using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ThingTable.ColumnWorkers;

public sealed class LabelColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, string> GetThingLabel =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            return thing.StuffDef == null
                ? thing.Def.LabelCap.RawText
                : $"{thing.StuffDef.LabelCap} {thing.Def.label}";
        });
    private LabelColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static LabelColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing.Def, thing.StuffDef).ToButtonSubtle(openDefInfoDialog),
                new Label(GetThingLabel(thing)),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget()
    {
        return new StringFilter<ThingAlike>(GetThingLabel);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }
}
