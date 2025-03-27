using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnDef
    : Def
{
    public StatDef? stat;
    public string? labelKey;
    public string? descriptionKey;
    public string? iconPath;
    public string? formatString;
    public StatValueExplanationType? statValueExplanationType;
#pragma warning disable CS8618
    public Type workerClass;
#pragma warning restore CS8618
    public bool isNegative = false;
    internal Texture2D? Icon { get; private set; }
#pragma warning disable CS8618
    internal IColumnWorker Worker { get; private set; }
#pragma warning restore CS8618
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            if (iconPath?.Length > 0)
            {
                Icon = ContentFinder<Texture2D>.Get(iconPath);
            }
        });
    }
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

        Worker = (IColumnWorker)Activator.CreateInstance(workerClass);
        Worker.ColumnDef = this;
    }
}
