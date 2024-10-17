﻿using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class ColumnDef : Def
{
    // Public API
    public StatDef? stat;
    public string labelKey = "";
    public string descriptionKey = "";
    public string icon = "";
    // Internal API
    internal string Label => LabelCap;
    internal string Description => description;
    internal TextAnchor CellTextAnchor;
    private Texture2D? _iconTex;
    internal Texture2D? Icon
    {
        get
        {
            if (_iconTex == null && icon?.Length > 0)
            {
                _iconTex = ContentFinder<Texture2D>.Get(icon);
            }

            return _iconTex;
        }
    }
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
        if (labelKey != "" && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey != "" && string.IsNullOrEmpty(description))
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
    internal abstract ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef);
}
