using System;
using System.Collections.Generic;
using Stats.Widgets;

namespace Stats.ThingTable;

public sealed class LabelColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, string> GetThingLabel =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            return thing.StuffDef == null
                ? thing.Def.LabelCap.RawText
                : $"{thing.StuffDef.LabelCap} {thing.Def.label}";
        });
    public LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing.Def, thing.StuffDef).ToButtonGhostly(openDefInfoDialog),
                new Label(GetThingLabel(thing)),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> _)
    {
        return new StringFilter<ThingAlike>(GetThingLabel);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }
}
