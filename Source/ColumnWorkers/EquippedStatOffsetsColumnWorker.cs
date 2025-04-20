using System.Text;
using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
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

        IWidget leftCol = new LabelWidget(labels.ToString());
        new TextAnchorWidgetComp(ref leftCol, TextAnchor.LowerLeft);

        IWidget rightCol = new LabelWidget(values.ToString());
        new WidgetComp_Width_Rel(ref rightCol, 1f);
        new TextAnchorWidgetComp(ref rightCol, TextAnchor.LowerRight);

        return new HorizontalContainerWidget([leftCol, rightCol], 10f, true);
    }
}
