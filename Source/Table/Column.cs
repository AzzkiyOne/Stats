using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

class Column
{
    public string? label { get; init; }
    public string? description { get; init; }
    public float minWidth { get; init; } = 100f;
    public bool isPinned { get; init; } = false;
    public bool isSortable { get; init; } = true;
    public virtual void DrawCell(Rect targetRect, Row row)
    {
        // Not very performant, because border will be rendered for each individual cell.
        GUIUtils.DrawLineVertical(
            targetRect.x + targetRect.width,
            targetRect.y,
            Table.rowHeight,
            Table.columnSeparatorLineColor
        );
    }
    public virtual bool DrawHeaderCell(Rect targetRect, SortDirection? sortDirection = null)
    {
        if (label != null)
        {
            Widgets.DrawHighlight(targetRect);
            Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), label);
        }

        if (sortDirection != null)
        {
            var rotationAngle = sortDirection == SortDirection.Ascending ? -90f : 90f;

            Widgets.DrawTextureRotated(targetRect.RightPartPixels(Table.headersRowHeight), TexButton.Reveal, rotationAngle);
        }

        GUIUtils.DrawLineVertical(targetRect.xMax, targetRect.y, targetRect.height, Table.columnSeparatorLineColor);

        if (Mouse.IsOver(targetRect) && description != null)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(description));
        }

        if (isSortable)
        {
            Widgets.DrawHighlightIfMouseover(targetRect);

            return Widgets.ButtonInvisible(targetRect);
        }
        else
        {
            return false;
        }
    }
    public virtual void SortRows(List<Row> rows, SortDirection direction)
    {
        rows.Sort((r1, r2) =>
        {
            var val1 = r1.GetCell(this, CreateCell).valueRaw;
            var val2 = r2.GetCell(this, CreateCell).valueRaw;

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
    protected virtual ICell CreateCell(ThingDef thingDef)
    {
        return new Cell();
    }
}

class LabelColumn : Column
{
    public LabelColumn() : base()
    {
        label = "Name";
        minWidth = 250f;
        isPinned = true;
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

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
        string labelText = row.thingDef.LabelCap == null ? row.thingDef.ToString() : row.thingDef.LabelCap;

        if (Debug.InDebugMode)
        {
            labelText = row.thingDef.defName;
        }

        Widgets.DefIcon(iconRect, row.thingDef);
        Widgets.LabelEllipses(textRect, labelText);

        if (Mouse.IsOver(targetRect))
        {
            Widgets.DrawHighlight(targetRect);

            if (Event.current.control)
            {
                TooltipHandler.TipRegion(targetRect, new TipSignal(row.thingDef.LabelCap + "\n\n" + row.thingDef.description));
            }
        }

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(row.thingDef));
        }
    }
    public override void SortRows(List<Row> rows, SortDirection direction)
    {
        if (direction == SortDirection.Ascending)
        {
            rows.Sort((r1, r2) => r1.thingDef.LabelCap.RawText.CompareTo(r2.thingDef.LabelCap.RawText));
        }
        else
        {
            rows.Sort((r1, r2) => r2.thingDef.LabelCap.RawText.CompareTo(r1.thingDef.LabelCap.RawText));
        }
    }
}

class StatColumn : Column
{
    public StatDef statDef { get; }
    public bool drawRawValue { get; init; } = false;
    public StatColumn(string statDefName) : base()
    {
        statDef = StatDef.Named(statDefName);
        label = statDef.LabelCap;
        description = statDef.description;
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var cell = row.GetCell(this, CreateCell);
        var text = Debug.InDebugMode || drawRawValue ? cell.valueRaw + "" : cell.valueDisplay;

        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), text);

        if (
            Mouse.IsOver(targetRect)
            && !string.IsNullOrEmpty(cell.valueExplanation)
            && Event.current.control
        )
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cell.valueExplanation));
        }
    }
    protected override ICell CreateCell(ThingDef thingDef)
    {
        return new StatCell(thingDef, statDef);
    }
}

class WeaponRangeColumn : Column
{
    public bool drawRawValue { get; init; } = true;
    public WeaponRangeColumn() : base()
    {
        label = "Range".Translate();
        description = "Stat_Thing_Weapon_Range_Desc".Translate();
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var cell = row.GetCell(this, CreateCell);
        var text = Debug.InDebugMode || drawRawValue ? cell.valueRaw + "" : cell.valueDisplay;

        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), text);

        if (
            Mouse.IsOver(targetRect)
            && !string.IsNullOrEmpty(cell.valueExplanation)
            && Event.current.control
        )
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cell.valueExplanation));
        }
    }
    protected override ICell CreateCell(ThingDef thingDef)
    {
        return new Cell()
        {
            valueRaw = thingDef.Verbs.First(v => v.isPrimary)?.range
        };
    }
}
