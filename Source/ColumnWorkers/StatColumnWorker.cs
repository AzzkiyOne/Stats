using System;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class StatColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private readonly Func<ThingAlike, float> GetStatValue;
    public StatColumnWorker()
    {
        GetStatValue = FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == false)
            {
                return default;
            }

            return ColumnDef.stat!.Worker.GetValue(statReq);
        });
    }
    private string FormatStatValue(float value, ThingAlike thing)
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
    private string? GetStatValueExplanation(float value, ThingAlike thing)
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
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetStatValue(thing);

        if (value == default || float.IsNaN(value))
        {
            return null;
        }

        var valueStr = FormatStatValue(value, thing);
        var tooltip = GetStatValueExplanation(value, thing);
        var widget = new Label(valueStr);

        if (tooltip?.Length > 0)
        {
            return widget.Tooltip(tooltip);
        }

        return widget;
    }
    public override FilterWidget GetFilterWidget()
    {
        return new NumberFilter<float>(GetStatValue);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetStatValue(thing1).CompareTo(GetStatValue(thing2));
    }
}
