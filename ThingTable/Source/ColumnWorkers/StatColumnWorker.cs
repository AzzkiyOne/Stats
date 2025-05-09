using RimWorld;
using Stats.ColumnWorkers;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public class StatColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    private const ToStringNumberSense toStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    private readonly StatValueExplanationType ExplanationType;
    protected StatColumnWorker(
        StatDef stat,
        StatValueExplanationType statValueExplanationType = StatValueExplanationType.None
    ) : base()
    {
        Stat = stat;
        ExplanationType = statValueExplanationType;
    }
    public static StatColumnWorker Make(ColumnDef columnDef) => new(
        columnDef.stat!,
        columnDef.statValueExplanationType
    );
    private string? GetStatValueExplanation(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);
        var statValue = worker.GetValue(statRequest);

        return ExplanationType switch
        {
            StatValueExplanationType.Full => worker.GetExplanationFull(statRequest, toStringNumberSense, statValue),
            StatValueExplanationType.Unfinalized => worker.GetExplanationUnfinalized(statRequest, toStringNumberSense),
            StatValueExplanationType.FinalizePart => worker.GetExplanationFinalizePart(statRequest, toStringNumberSense, statValue),
            _ => "",
        };
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);

        if (worker.ShouldShowFor(statRequest) == false)
        {
            return "";
        }

        var statValue = worker.GetValue(statRequest);

        return worker.GetStatDrawEntryLabel(Stat, statValue, toStringNumberSense, statRequest);
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
