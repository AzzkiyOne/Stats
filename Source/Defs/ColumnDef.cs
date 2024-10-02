using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class ColumnDef : Def
{
    // Public API
    public StatDef? stat;
    public string? labelKey;
    public string? descriptionKey;
    public float minWidth = 50f;
    public bool bestIsHighest = true;
    // Internal API
    internal string Label => LabelCap;
    internal string Description => description;
    private float? _minWidth = null;
    /// <summary>
    /// This should only be accessed in GUI context. Otherwise the game will crash.
    /// </summary>
    internal float MinWidth => _minWidth ??= Math.Max(Text.CalcSize(label).x + 15f, minWidth);
    internal bool BestIsHighest => bestIsHighest;
    internal TextAnchor CellTextAnchor { get; }
    internal ColumnDef(ColumnStyle style = ColumnStyle.String)
    {
        CellTextAnchor = style switch
        {
            ColumnStyle.Number => TextAnchor.LowerRight,
            ColumnStyle.Boolean => TextAnchor.LowerCenter,
            _ => TextAnchor.LowerLeft,
        };
    }
    public override void ResolveReferences()
    {
        if (labelKey != null && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey != null && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }

        if (stat != null)
        {
            if (string.IsNullOrEmpty(label))
            {
                label = stat.label;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = stat.description;
            }
        }
    }
    internal abstract ICellWidget? GetCellWidget(ThingAlike thing);
}
