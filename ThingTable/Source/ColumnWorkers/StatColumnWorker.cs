using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class StatColumnWorker : ColumnWorker<ThingAlike>
{
    private readonly StatDef Stat;
    private readonly string? FormatString;
    private readonly StatValueExplanationType ExplanationType;
    private static readonly Regex NumRegex = new(@"[0-9]+\.?[0-9]+", RegexOptions.Compiled);
    private static readonly Regex NonZeroNumRegex = new(@"[1-9]+", RegexOptions.Compiled);
    private readonly Func<ThingAlike, decimal> GetDisplayedStatValue;
    private StatColumnWorker(ColumnDef columnDef) : base(TableColumnCellStyle.Number)
    {
        Stat = columnDef.stat!;
        FormatString = columnDef.statValueFormatString;
        ExplanationType = columnDef.statValueExplanationType;
        GetDisplayedStatValue = new Func<ThingAlike, decimal>((thing) =>
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
    public static StatColumnWorker Make(ColumnDef columnDef) => new(columnDef);
    private float GetStatValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (Stat.Worker.ShouldShowFor(statReq) == false)
        {
            return 0f;
        }

        return Stat.Worker.GetValue(statReq);
    }
    private string FormatStatValue(float value, ThingAlike thing)
    {
        if (FormatString != null)
        {
            return value.ToString(FormatString);
        }

        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return Stat.Worker.GetStatDrawEntryLabel(Stat, value, ToStringNumberSense.Absolute, statReq);
    }
    private string GetStatValueExplanation(float value, ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return ExplanationType switch
        {
            StatValueExplanationType.Full =>
                Stat.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, value),
            StatValueExplanationType.Unfinalized =>
                Stat.Worker.GetExplanationUnfinalized(statReq, ToStringNumberSense.Absolute),
            StatValueExplanationType.FinalizePart =>
                Stat.Worker.GetExplanationFinalizePart(statReq, ToStringNumberSense.Absolute, value),
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
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> _)
    {
        return new NumberFilter<ThingAlike>(GetDisplayedStatValue);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetDisplayedStatValue(thing1).CompareTo(GetDisplayedStatValue(thing2));
    }
}
