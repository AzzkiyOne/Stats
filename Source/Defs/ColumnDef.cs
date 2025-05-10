using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class ColumnDef : Def, IColumnDef
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
    public IColumnDef.LabelFormatter? labelFormat;
    public IColumnDef.LabelFormatter LabelFormat => labelFormat!;
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
