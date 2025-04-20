using RimWorld;
using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Widgets;
using Verse;

namespace Stats.ColumnWorkers.Generic;

public class StatColumnWorker
    : ColumnWorker<float>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Number;
    protected override float GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == false)
        {
            return 0f;
        }

        return ColumnDef.stat!.Worker.GetValue(statReq);
    }
    protected override bool ShouldShowValue(float value)
    {
        return value != 0f && float.IsNaN(value) == false;
    }
    protected virtual string FormatValue(float value, ThingAlike thing)
    {
        if (ColumnDef.formatString != null)
        {
            return value.ToString(ColumnDef.formatString);
        }

        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
            ColumnDef.stat!,
            value,
            ToStringNumberSense.Absolute,
            statReq
        );
    }
    protected virtual string? GetValueExplanation(float value, ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return ColumnDef.statValueExplanationType switch
        {
            StatValueExplanationType.Full =>
                ColumnDef.stat!.Worker.GetExplanationFull(
                    statReq,
                    ToStringNumberSense.Absolute,
                    value
                ),
            StatValueExplanationType.Unfinalized =>
                ColumnDef.stat!.Worker.GetExplanationUnfinalized(
                    statReq,
                    ToStringNumberSense.Absolute
                ),
            StatValueExplanationType.FinalizePart =>
                ColumnDef.stat!.Worker.GetExplanationFinalizePart(
                    statReq,
                    ToStringNumberSense.Absolute,
                    value
                ),
            _ => null,
        };
    }
    protected override IWidget GetTableCellContent(float value, ThingAlike thing)
    {
        var valueStr = FormatValue(value, thing);
        var tooltip = GetValueExplanation(value, thing);
        IWidget widget = new LabelWidget(valueStr);

        if (tooltip?.Length > 0)
        {
            new TooltipWidgetComp(ref widget, tooltip);
        }

        return widget;
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new NumberFilterWidget<float>(GetValueCached);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
