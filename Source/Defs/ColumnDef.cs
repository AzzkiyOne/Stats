using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public StatDef? stat;
    public string? labelKey;
    public string? descriptionKey;
    public string? iconPath;
    public string? formatString;
    public StatValueExplanationType? statValueExplanationType;
    public Type workerClass;
    public bool bestIsHighest = true;
    internal Texture2D? Icon { get; private set; }
    internal IColumnWorker Worker { get; private set; }
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
