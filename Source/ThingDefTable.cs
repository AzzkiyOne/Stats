using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class ThingDefTable
{
    public static readonly List<ThingDefTable_Column> columns;
    public static readonly List<Dictionary<string, ThingDefTable_Cell>> rows;
    static ThingDefTable()
    {
        // Columns
        var statDefs = DefDatabase<StatDef>.AllDefs;
        var columns = new List<ThingDefTable_Column>(statDefs.Count());

        // From StatDef's
        foreach (var statDef in statDefs)
        {
            columns.Add(new ThingDefTable_StatDefColumn(statDef));
        }

        // Custom columns
        columns.Add(new ThingDefTable_WeaponRangeColumn());

        ThingDefTable.columns = columns;

        // Rows
        var thingDefs = DefDatabase<ThingDef>.AllDefs;
        var rows = new List<Dictionary<string, ThingDefTable_Cell>>(thingDefs.Count());

        foreach (var thingDef in thingDefs)
        {
            var row = new Dictionary<string, ThingDefTable_Cell>(columns.Count);

            foreach (var column in columns)
            {
                row.Add(column.id, column.GetCellFor(thingDef));
            }

            rows.Add(row);
        }

        ThingDefTable.rows = rows;
    }
}

public class ThingDefTable_Cell(
    float? valueRaw = null,
    string? valueString = null,
    string? valueExplanation = null,
    Texture2D? valueTexture = null
)
{
    public readonly float? valueRaw = valueRaw;
    public readonly string valueString = valueString ?? "";
    public readonly string valueExplanation = valueExplanation ?? "";
    public readonly Texture2D valueTexture = valueTexture ?? Widgets.PlaceholderIconTex;

    public static readonly ThingDefTable_Cell Empty = new();
}

public abstract class ThingDefTable_Column(
    string id,
    string? label = null,
    string? description = null
)
{
    public readonly string id = id;
    public readonly string label = label ?? "";
    public readonly string description = description ?? "";

    public abstract ThingDefTable_Cell GetCellFor(ThingDef thingDef);
}

public class ThingDefTable_StatDefColumn(StatDef statDef) : ThingDefTable_Column(
    statDef.defName,
    statDef.LabelCap,
    statDef.description
)
{
    public override ThingDefTable_Cell GetCellFor(ThingDef thingDef)
    {
        var statReq = StatRequest.For(thingDef, thingDef.defaultStuff);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return ThingDefTable_Cell.Empty;
        }

        float? valueRaw = null;
        string? valueString = null;
        string? valueExplanation = null;

        // This is all very expensive.
        // The good thing is that it all will be cached.
        try
        {
            valueRaw = thingDef.GetStatValueAbstract(statDef);
        }
        catch
        {
        }

        if (valueRaw is float _valueRaw)
        {
            try
            {
                //Why ToStringNumberSense.Absolute?
                valueString = statDef.Worker.GetStatDrawEntryLabel(statDef, _valueRaw, ToStringNumberSense.Absolute, statReq);
            }
            catch
            {
            }

            try
            {
                // Why valueRaw as final value?
                valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, _valueRaw);
            }
            catch
            {
            }
        }

        return new ThingDefTable_Cell(
            valueRaw: valueRaw,
            valueString: valueString,
            valueExplanation: valueExplanation
        );
    }
}

public class ThingDefTable_WeaponRangeColumn() : ThingDefTable_Column(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    public override ThingDefTable_Cell GetCellFor(ThingDef thingDef)
    {
        return new ThingDefTable_Cell(
            valueRaw: thingDef.Verbs.First(v => v.isPrimary)?.range
        );
    }
}