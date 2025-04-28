using System.Text;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class EquippedStatOffsetsColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static int GetOffsetsCount(ThingAlike thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? default;
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var statOffsetsCount = GetOffsetsCount(thing);

        if (statOffsetsCount == default)
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
    public override FilterWidget GetFilterWidget()
    {
        return new NumberFilter<int>(GetOffsetsCount);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetOffsetsCount(thing1).CompareTo(GetOffsetsCount(thing2));
    }
}
