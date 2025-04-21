using System.Text;
using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Containers;
using Stats.Widgets.Extensions;
using Stats.Widgets.Extensions.Size.Constraints;
using Stats.Widgets.Misc;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers;

public class EquippedStatOffsetsColumnWorker
    : NumberColumnWorker<float>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    protected override float GetValue(ThingAlike thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override IWidget GetTableCellContent(float value, ThingAlike thing)
    {
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
            10f,
            true
        );
    }
}
