using RimWorld;
using Stats.ColumnWorkers;
using UnityEngine;
using Verse;

namespace Stats.Defs;

public abstract class ColumnDef : Def, IColumnDef
{
    public string? labelKey;
    public string? descriptionKey;
    public string labelShort = "";
    public string LabelShort => labelShort;
    public string Description => description;
    public string? iconPath;
    public Texture2D? Icon { get; private set; }
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
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

        if (string.IsNullOrEmpty(labelShort))
        {
            labelShort = LabelCap;
        }
    }
    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);
        }
    }
}

public interface IColumnDef
{
    string LabelShort { get; }
    TaggedString LabelCap { get; }
    string Description { get; }
    public Texture2D? Icon { get; }
}

public interface IColumnDef<T> : IColumnDef
{
    ColumnWorker<T> Worker { get; }
}
