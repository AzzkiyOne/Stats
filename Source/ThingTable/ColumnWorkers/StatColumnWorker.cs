using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public class StatColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    private readonly StatValueExplanationType ExplanationType;
    public StatColumnWorker(StatColumnDef columnDef) : base(columnDef)
    {
        Stat = columnDef.stat;
        ExplanationType = columnDef.statValueExplanationType;
    }
    private string? GetStatValueExplanation(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);
        var statValue = worker.GetValue(statRequest);

        return ExplanationType switch
        {
            StatValueExplanationType.Full => worker.GetExplanationFull(statRequest, _ToStringNumberSense, statValue),
            StatValueExplanationType.Unfinalized => worker.GetExplanationUnfinalized(statRequest, _ToStringNumberSense),
            StatValueExplanationType.FinalizePart => worker.GetExplanationFinalizePart(statRequest, _ToStringNumberSense, statValue),
            _ => "",
        };
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);

        if (worker.ShouldShowFor(statRequest))
        {
            var statValue = worker.GetValue(statRequest);

            if (statValue != 0f)
            {
                return worker.GetStatDrawEntryLabel(Stat, statValue, _ToStringNumberSense, statRequest);
            }
        }

        return "";
    }
    public sealed override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var widget = base.GetTableCellWidget(thing);

        if (widget != null && ExplanationType != StatValueExplanationType.None)
        {
            var tooltip = GetStatValueExplanation(thing);

            if (tooltip?.Trim().Length > 0)
            {
                return widget.Tooltip(tooltip);
            }
        }

        return widget;
    }
}
