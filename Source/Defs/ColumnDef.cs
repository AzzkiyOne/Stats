using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public ColumnStyle style = ColumnStyle.Number;
    internal TextAnchor CellTextAnchor { get; private set; }
    public StatDef? stat;
    public string? labelKey;
    public string? descriptionKey;
    public string? icon;
    public string? formatString;
    public Type workerClass;
    private Texture2D? _iconTex;
    public Texture2D? Icon
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
    private IColumnWorker _worker;
    public IColumnWorker Worker
    {
        get
        {
            if (_worker == null)
            {
                _worker = (IColumnWorker)Activator.CreateInstance(workerClass);
                _worker.ColumnDef = this;
            }

            return _worker;
        }
    }
    public override void ResolveReferences()
    {
        if (labelKey?.Length > 0 && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey?.Length > 0 && string.IsNullOrEmpty(description))
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

        CellTextAnchor = style switch
        {
            ColumnStyle.Number => TextAnchor.LowerRight,
            ColumnStyle.Boolean => TextAnchor.LowerCenter,
            _ => TextAnchor.LowerLeft,
        };
    }
}
