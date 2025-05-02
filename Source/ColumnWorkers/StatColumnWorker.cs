using System;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class StatColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private static readonly Regex NumRegex = new(@"[0-9]+\.?[0-9]+", RegexOptions.Compiled);
    private static readonly Regex NonZeroNumRegex = new(@"[1-9]+", RegexOptions.Compiled);
    private readonly Func<ThingAlike, decimal> GetDisplayedStatValue;
    public StatColumnWorker()
    {
        GetDisplayedStatValue = new Func<ThingAlike, decimal>((ThingAlike thing) =>
        {
            var statValue = GetStatValue(thing);

            if (statValue == 0f)
            {
                return 0m;
            }

            var formattedStatValue = FormatStatValue(statValue, thing);
            var match = NumRegex.Match(formattedStatValue);

            if (match.Success)
            {
                var numWasParsed = decimal.TryParse(match.Value, out var num);

                if (numWasParsed)
                {
                    return num;
                }
            }

            // TODO: Maybe just throw an exception here.
            return (decimal)statValue;
        })
        .Memoized();
    }
    private float GetStatValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == false)
        {
            return 0f;
        }

        return ColumnDef.stat!.Worker.GetValue(statReq);
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
    private string GetStatValueExplanation(float value, ThingAlike thing)
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
            _ => "",
        };
    }
    private static bool IsZeroString(string value)
    {
        return NonZeroNumRegex.IsMatch(value) == false;
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetStatValue(thing);

        if (float.IsNaN(value))
        {
            return null;
        }

        var valueFormatted = FormatStatValue(value, thing);

        if (IsZeroString(valueFormatted))
        {
            return null;
        }

        Widget widget = new Label(valueFormatted);

        var tooltip = GetStatValueExplanation(value, thing);
        if (tooltip.Length > 0)
        {
            widget = widget.Tooltip(tooltip);
        }

        return widget;
    }
    public override FilterWidget GetFilterWidget()
    {
        return new NumberFilter(GetDisplayedStatValue);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetDisplayedStatValue(thing1).CompareTo(GetDisplayedStatValue(thing2));
    }
}
