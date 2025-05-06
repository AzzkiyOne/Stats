using System.Collections.Generic;
using System.Text;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ThingTable;

public sealed class EquippedStatOffsetsColumnWorker : ColumnWorker<ThingAlike>
{
    private EquippedStatOffsetsColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static EquippedStatOffsetsColumnWorker Make(ColumnDef _) => new();
    private static decimal GetOffsetsCount(ThingAlike thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0m;
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var statOffsetsCount = GetOffsetsCount(thing);

        if (statOffsetsCount == 0m)
        {
            return null;
        }

        var labels = new StringBuilder();
        var values = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetLabel = $"{offset.stat.LabelCap}:";
            var offsetValueStr = offset.stat.Worker.ValueToString(
                offset.value,
                true,
                ToStringNumberSense.Offset
            );

            labels.AppendInNewLine(offsetLabel);
            values.AppendInNewLine(offsetValueStr);
        }

        return new HorizontalContainer(
            [
                new Label(labels.ToString())
                    .TextAnchor(TextAnchor.LowerLeft),
                new Label(values.ToString())
                    .WidthRel(1f)
                    .TextAnchor(TextAnchor.LowerRight),
            ],
            Globals.GUI.Pad,
            true
        );
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> _)
    {
        return new NumberFilter<ThingAlike>(GetOffsetsCount);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetOffsetsCount(thing1).CompareTo(GetOffsetsCount(thing2));
    }
}
