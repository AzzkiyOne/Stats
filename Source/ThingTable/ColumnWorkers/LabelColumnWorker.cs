using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class LabelColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, string> GetThingLabel =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            // Note to myself: Don't remove stuff label. It's important because
            // modded stuffs may have the same color as vanilla ones or other modded stuffs.
            // Replacing label with icon won't do, because ex. all of the leathers have the same
            // icon but of different color.
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
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var labelFilter = new StringFilter<ThingAlike>(GetThingLabel);

        if (tableRecords.Any(record => record.StuffDef != null))
        {
            var stuffDefs = tableRecords
                .Select(thing => thing.StuffDef)
                .Distinct()
                .OrderBy(stuffDef => stuffDef?.label);
            var stuffFilter = new OneToManyFilter<ThingAlike, ThingDef?>(
                thing => thing.StuffDef,
                stuffDefs,
                stuffDef => stuffDef == null
                    ? new Label("")
                    : new HorizontalContainer(
                        [
                            new ThingIcon(stuffDef),
                        new Label(stuffDef.LabelCap).WidthRel(1f)
                        ],
                        Globals.GUI.PadSm,
                        true
                    )
            );

            return new CompositeFilter<ThingAlike>(
                stuffFilter.Tooltip("Filter by stuff."),
                labelFilter.WidthRel(1f).Tooltip("Filter by label.")
            );
        }

        return labelFilter;
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }
}
