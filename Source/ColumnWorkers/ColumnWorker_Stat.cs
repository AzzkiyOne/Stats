using RimWorld;
using Verse;

namespace Stats;

public class ColumnWorker_Stat
    : ColumnWorker<float>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Number;
    public override float GetValue(ThingRec thing)
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
    protected virtual string FormatValue(float value, ThingRec thing)
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
    protected virtual string? GetValueExplanation(float value, ThingRec thing)
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
    protected override IWidget GetTableCellContent(float value, ThingRec thing)
    {
        var valueStr = FormatValue(value, thing);
        var tooltip = GetValueExplanation(value, thing);
        IWidget widget = new Widget_Label(valueStr);

        if (tooltip?.Length > 0)
        {
            new WidgetComp_Tooltip(ref widget, tooltip);
        }

        return widget;
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Num(new(GetValue));
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
