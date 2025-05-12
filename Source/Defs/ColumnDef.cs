using System;
using RimWorld;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public string? labelKey;
    public string labelShort = "";
    public string LabelShort => labelShort;
    public string? descriptionKey;
    public string Description => description;
    public string? iconPath;
    public Texture2D? Icon { get; private set; }
    public Color iconColor = Color.white;
    public Color IconColor => iconColor;
    public float iconScale = 1f;
    public float IconScale => iconScale;
    public LabelFormatter? labelFormat;
    public LabelFormatter LabelFormat => labelFormat!;
    // Why not to make it a method of ColumnWorker?
    //
    // It doesn't fit there semantically.
    // ColumnWorker is about data, ColumnDef is about column in general.
    public delegate Widget LabelFormatter(ColumnDef columnDef, ColumnCellStyle columnStyle);
#pragma warning disable CS8618
    public Type workerClass;
    public ColumnWorker Worker { get; private set; }
#pragma warning restore CS8618
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (labelKey?.Length > 0 && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey?.Length > 0 && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }

        if (string.IsNullOrEmpty(labelShort))
        {
            labelShort = LabelCap;
        }

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            ResolveIcon();
            Worker = (ColumnWorker)Activator.CreateInstance(workerClass, this);
        });
    }
    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);

            if (labelFormat == null)
            {
                labelFormat = ColumnLabelFormat.LabelWithIcon;
            }
        }
        else
        {
            labelFormat = ColumnLabelFormat.LabelOnly;
        }
    }
}
