using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
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
    private static readonly Func<ThingAlike, string> GetOffsetsString =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        if (GetOffsetsCount(thing) == 0)
        {
            return "";
        }

        var result = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            result.Append(offset.stat.LabelCap);
            result.Append(GetOffsetValueString(offset));
        }

        return result.ToString();
    });
    private static int GetOffsetsCount(ThingAlike thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0;
    }
    private static string GetOffsetValueString(StatModifier offset)
    {
        return offset.stat.ValueToString(
            offset.value,
            ToStringNumberSense.Offset,
            offset.stat.finalizeEquippedStatOffset
        );
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        if (GetOffsetsCount(thing) == 0)
        {
            return null;
        }

        var labels = new StringBuilder();
        var values = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetLabel = $"{offset.stat.LabelCap}:";
            var offsetValueStr = GetOffsetValueString(offset);

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
        return new StringFilter<ThingAlike>(GetOffsetsString);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetOffsetsCount(thing1).CompareTo(GetOffsetsCount(thing2));
    }
}
