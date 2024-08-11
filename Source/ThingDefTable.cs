global using ThingDefTable_Row = System.Collections.Generic.Dictionary<string, Stats.ThingDefTable_Cell>;

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

//[StaticConstructorOnStartup]
public static class ThingDefTable
{
    public static Dictionary<string, ThingDefTable_Column> columns;
    public static Dictionary<string, ThingDefTable_Column> customColumns;
    public static Dictionary<ThingDef, ThingDefTable_Row> rows;
    static ThingDefTable()
    {
        customColumns = new()
        {
            ["ThingDefRef"] = new ThingDefTable_ThingDefRefColumn(),
            ["Range"] = new ThingDefTable_WeaponRangeColumn(),
        };

        // Columns
        var statDefs = DefDatabase<StatDef>.AllDefs;
        var columns = new Dictionary<string, ThingDefTable_Column>(
            statDefs.Count() + customColumns.Count
        )
        {
            ["MaxHitPoints"] = new ThingDefTable_StatDefColumn(StatDefOf.MaxHitPoints, "HP"),
            ["MarketValue"] = new ThingDefTable_StatDefColumn(StatDefOf.MarketValue, "$"),
        };

        // From StatDef's
        foreach (var statDef in statDefs)
        {
            var column = new ThingDefTable_StatDefColumn(statDef);

            columns.TryAdd(column.id, column);
        }

        // Custom columns
        foreach (var column in customColumns.Values)
        {
            columns.TryAdd(column.id, column);
        }

        ThingDefTable.columns = columns;

        // Rows
        var thingDefs = DefDatabase<ThingDef>.AllDefs;
        var rows = new Dictionary<ThingDef, ThingDefTable_Row>(thingDefs.Count());

        foreach (var thingDef in thingDefs)
        {
            var row = new ThingDefTable_Row(columns.Count);

            foreach (var column in columns.Values)
            {
                row.Add(column.id, column.GetCellFor(thingDef));
            }

            rows.Add(thingDef, row);
        }

        ThingDefTable.rows = rows;
    }
}

public class ThingDefTable_Cell(
    float? valueNum = null,
    string? valueString = null,
    string? valueExplanation = null,
    Def? valueDef = null
)
{
    public readonly float? valueNum = valueNum;
    public readonly string valueString = valueString ?? "";
    public readonly string valueExplanation = valueExplanation ?? "";
    public readonly Def? valueDef = valueDef;

    public virtual void Draw(Rect targetRect)
    {
        // Not very performant, because border will be rendered for each individual cell.
        //GUIUtils.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    Table.rowHeight,
        //    Table.columnSeparatorLineColor
        //);
    }

    public static readonly ThingDefTable_Cell Empty = new();
}

public class ThingDefTable_NumCell(
    float? valueNum = null,
    string? valueString = null,
    string? valueExplanation = null
) : ThingDefTable_Cell(
    valueNum,
    valueString,
    valueExplanation
)
{
    public override void Draw(Rect targetRect)
    {
        base.Draw(targetRect);

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);
        var text = Debug.InDebugMode ? valueNum + "" : valueString;

        Widgets.Label(contentRect, text);

        if (Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(valueExplanation));
        }
    }
}

public class ThingDefTable_RefCell(
    float? valueNum = null,
    string? valueString = null,
    string? valueExplanation = null,
    Def? valueDef = null
) : ThingDefTable_Cell(
    valueNum,
    valueString,
    valueExplanation,
    valueDef
)
{
    public override void Draw(Rect targetRect)
    {
        base.Draw(targetRect);

        if (valueDef == null)
        {
            return;
        }

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);
        var iconRect = new Rect(
            contentRect.x,
            contentRect.y,
            contentRect.height,
            contentRect.height
        );
        var textRect = new Rect(
            iconRect.xMax + Table.cellPaddingHor,
            contentRect.y,
            contentRect.width - iconRect.width - Table.cellPaddingHor,
            contentRect.height
        );
        string labelText = Debug.InDebugMode ? valueDef.defName : valueString;

        Widgets.DefIcon(iconRect, valueDef);
        Widgets.Label(textRect, labelText);
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(valueExplanation));
        }

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(valueDef));
        }
    }
}

public abstract class ThingDefTable_Column(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
)
{
    public readonly string id = id;
    public readonly string label = label ?? "";
    public readonly string description = description ?? "";
    public readonly float minWidth = minWidth ?? 100f;

    public bool Draw(Rect targetRect, SortDirection? sortDirection = null)
    {
        Widgets.DrawHighlight(targetRect);
        Widgets.Label(targetRect.ContractedBy(Table.cellPaddingHor, 0), label);

        if (sortDirection != null)
        {
            var rotationAngle = sortDirection == SortDirection.Ascending ? -90f : 90f;

            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(Table.headersRowHeight),
                TexButton.Reveal,
                rotationAngle
            );
        }

        //GUIUtils.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    targetRect.height,
        //    Table.columnSeparatorLineColor
        //);

        TooltipHandler.TipRegion(targetRect, new TipSignal(description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        return Widgets.ButtonInvisible(targetRect);
    }

    public abstract ThingDefTable_Cell GetCellFor(ThingDef thingDef);
    // This is a part of abstraction from column data type.
    // Because our columns don't really have fixed data types.
    public abstract void SortRows(List<ThingDefTable_Row> rows, SortDirection direction);
}

public abstract class ThingDefTable_NumColumn(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
) : ThingDefTable_Column(
    id,
    label,
    description,
    minWidth
)
{
    public override void SortRows(List<ThingDefTable_Row> rows, SortDirection direction)
    {
        rows.Sort((r1, r2) =>
        {
            var val1 = r1.TryGetValue(id)?.valueNum;
            var val2 = r2.TryGetValue(id)?.valueNum;

            if (val1 == val2)
            {
                return 0;
            }
            else if (val1 == null || val2 == null)
            {
                return -1;
            }
            else if (direction == SortDirection.Ascending)
            {
                return val1 > val2 ? 1 : -1;
            }
            else
            {
                return val1 < val2 ? 1 : -1;
            }
        });
    }
}

public abstract class ThingDefTable_StringColumn(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
) : ThingDefTable_Column(
    id,
    label,
    description,
    minWidth
)
{
    public override void SortRows(List<ThingDefTable_Row> rows, SortDirection direction)
    {
        if (direction == SortDirection.Ascending)
        {
            rows.Sort((r1, r2) =>
            {
                var val1 = r1.TryGetValue(id)?.valueString;
                var val2 = r2.TryGetValue(id)?.valueString;

                return val1.CompareTo(val2);
            });
        }
        else
        {
            rows.Sort((r1, r2) =>
            {
                var val1 = r1.TryGetValue(id)?.valueString;
                var val2 = r2.TryGetValue(id)?.valueString;

                return val2.CompareTo(val1);
            });
        }
    }
}

public class ThingDefTable_StatDefColumn(
    StatDef statDef,
    string? label = null,
    float? minWidth = null
) : ThingDefTable_NumColumn(
    statDef.defName,
    label ?? statDef.LabelCap,
    statDef.description,
    minWidth
)
{
    public override ThingDefTable_Cell GetCellFor(ThingDef thingDef)
    {
        var statReq = StatRequest.For(thingDef, thingDef.defaultStuff);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return ThingDefTable_Cell.Empty;
        }

        float? valueNum = null;
        string? valueString = null;
        string? valueExplanation = null;

        // This is all very expensive.
        // The good thing is that it all will be cached.
        try
        {
            valueNum = thingDef.GetStatValueAbstract(statDef);
        }
        catch
        {
        }

        if (valueNum is float _valueNum)
        {
            try
            {
                // Why ToStringNumberSense.Absolute?
                valueString = statDef.Worker.GetStatDrawEntryLabel(statDef, _valueNum, ToStringNumberSense.Absolute, statReq);
            }
            catch
            {
            }

            try
            {
                // Why valueNum as final value?
                valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, _valueNum);
            }
            catch
            {
            }
        }

        return new ThingDefTable_NumCell(
            valueNum: valueNum,
            valueString: valueString,
            valueExplanation: valueExplanation
        );
    }
}

public class ThingDefTable_WeaponRangeColumn() : ThingDefTable_NumColumn(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    public override ThingDefTable_Cell GetCellFor(ThingDef thingDef)
    {
        if (thingDef.Verbs.Count == 0)
        {
            return ThingDefTable_Cell.Empty;
        }

        var value = thingDef.Verbs.First(v => v.isPrimary)?.range;

        return new ThingDefTable_NumCell(
            valueNum: value,
            valueString: value + ""
        );
    }
}

public class ThingDefTable_ThingDefRefColumn() : ThingDefTable_StringColumn(
    "ThingDefRef",
    "Name",
    minWidth: 250f
)
{
    public override ThingDefTable_Cell GetCellFor(ThingDef thingDef)
    {
        return new ThingDefTable_RefCell(
            valueString: thingDef.LabelCap,
            valueExplanation: thingDef.LabelCap + "\n\n" + thingDef.description,
            valueDef: thingDef
        );
    }
}