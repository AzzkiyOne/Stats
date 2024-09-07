using RimWorld;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

public interface IColumnDefWithWorker<DataType>
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public ColumnType Type { get; }
    public TextAnchor TextAnchor { get; }
    public bool ReverseDiffModeColors { get; }
    public bool IsPinned { get; }
    public IColumnWorker<DataType> Worker { get; }
}

public abstract class ColumnDef : Def
{
    public string Label => LabelCap;
    public string? labelKey;
    public string Description => description;
    public string? descriptionKey;
    public float minWidth = 75f;
    public float MinWidth => minWidth;
    public ColumnType type = ColumnType.Number;
    public ColumnType Type => type;
    public TextAnchor textAnchor;
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
}

public enum ColumnType
{
    Number,
    String,
    Boolean,
}