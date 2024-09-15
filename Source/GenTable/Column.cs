using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

public interface IColumn<DataType>
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public bool IsPinned { get; }
    public TextAnchor TextAnchor { get; }
    public bool ReverseDiffModeColors { get; }
    ICell? GetCell(DataType data);
}

public abstract class ColumnDef : Def
{
    public string Label => LabelCap;
    public string? labelKey;
    public string Description => description;
    public string? descriptionKey;
    public float minWidth = 75f;
    private float? adjMinWidth = null;
    /// <summary>
    /// This should only be accessed in GUI context. Otherwise the game will crash.
    /// </summary>
    public float MinWidth => adjMinWidth ??= Math.Max(Text.CalcSize(label).x + 15f, minWidth);
    public ColumnType type = ColumnType.Number;
    public ColumnType Type => type;
    private TextAnchor textAnchor;
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

public class Column_Label : ColumnDef, IColumn<GeneDef>, IColumn<ThingDefTable.ThingAlike>
{
    public ICell? GetCell(ThingDefTable.ThingAlike thing)
    {
        return new Cell_DefRef(thing.Def, thing.Stuff);
    }
    public ICell? GetCell(GeneDef gene)
    {
        return new Cell_DefRef(gene);
    }
}

[DefOf]
public static class ColumnDefOf
{
    public static Column_Label Label;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}