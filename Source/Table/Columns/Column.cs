using System;
using UnityEngine;
using Verse;

namespace Stats.Table.Columns;

public abstract class Column : Def, IColumn
{
    public string Label => LabelCap;
    public string? labelKey;
    public string Description => description;
    public string? descriptionKey;
    public float minWidth = 50f;
    internal float? adjMinWidth = null;
    /// <summary>
    /// This should only be accessed in GUI context. Otherwise the game will crash.
    /// </summary>
    public float MinWidth => adjMinWidth ??= Math.Max(Text.CalcSize(label).x + 15f, minWidth);
    public ColumnType type = ColumnType.Number;
    public ColumnType Type => type;
    internal TextAnchor textAnchor;
    public TextAnchor TextAnchor => textAnchor;
    public bool reverseDiffModeColors = false;
    public bool ReverseDiffModeColors => reverseDiffModeColors;
    public bool isPinned = false;
    public bool IsPinned => isPinned;
    public override void ResolveReferences()
    {
        textAnchor = type == ColumnType.Number
            ? TextAnchor.LowerRight
            : type == ColumnType.Boolean
            ? TextAnchor.LowerCenter
            : TextAnchor.LowerLeft;

        if (labelKey != null && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey != null && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }
    }
    public abstract ICell? GetCell(ThingAlike thing);
}
