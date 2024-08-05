using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

class Table
{
    public const float rowHeight = 30f;
    public const float headersRowHeight = rowHeight;
    public const float cellPaddingHor = 10f;
    public static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);
    public Vector2 scrollPosition = new();
    public readonly List<ColumnDef> columns = [];
    public readonly List<ColumnDef> pinnedColumns = [];
    readonly List<Row> rows;
    readonly float columnsWidth = 0f;
    readonly float pinnedColumnsWidth = 0f;
    readonly float minRowWidth = 0f;
    readonly float totalRowsHeight = 0f;
    int? mouseOverRowIndex = null;
    public Table(List<ColumnDef> columns, List<Row> rows)
    {
        this.rows = rows;

        foreach (var column in columns)
        {
            if (column.isPinned)
            {
                pinnedColumns.Add(column);
                pinnedColumnsWidth += column.minWidth;
            }
            else
            {
                this.columns.Add(column);
                columnsWidth += column.minWidth;
            }

            minRowWidth += column.minWidth;
        }

        totalRowsHeight = rowHeight * rows.Count;
    }
    public void Draw(Rect targetRect)
    {
        Text.Font = GameFont.Small;
        TextAnchor prevTextAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;

        var contentRect = new Rect(0f, 0f, minRowWidth, totalRowsHeight + headersRowHeight);

        Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

        // Draw pinned headers
        var pinnedHeadersRowRect = new Rect(scrollPosition.x, scrollPosition.y, pinnedColumnsWidth, headersRowHeight);
        DrawHeaders(pinnedHeadersRowRect, pinnedColumns);

        // Draw headers
        var headersRowRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y, contentRect.xMax - pinnedColumnsWidth, rowHeight);
        DrawHeaders(headersRowRect, columns, scrollPosition);

        // Draw pinned rows
        var pinnedTableBodyRect = new Rect(scrollPosition.x, scrollPosition.y + headersRowHeight, pinnedColumnsWidth, targetRect.height - headersRowHeight);
        DrawRows(pinnedTableBodyRect, pinnedColumns, new Vector2(0, scrollPosition.y));

        // Draw rows
        //var tableBodyRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y + headersRowHeight, totalColumnsWidth, contentRect.height - headersRowHeight);
        var tableBodyRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y + headersRowHeight, targetRect.width - pinnedColumnsWidth, targetRect.height - headersRowHeight);
        DrawRows(tableBodyRect, columns, scrollPosition);

        // Separators
        Utils.DrawLineVertical(scrollPosition.x, 0f, contentRect.height, new(1f, 1f, 1f, 0.4f));
        Widgets.DrawLineHorizontal(0f, headersRowHeight + scrollPosition.y, contentRect.width, new(1f, 1f, 1f, 0.4f));
        Utils.DrawLineVertical(pinnedColumnsWidth + scrollPosition.x, 0f, contentRect.height, new(1f, 1f, 1f, 0.4f));

        if (!Mouse.IsOver(pinnedTableBodyRect.Union(tableBodyRect)))
        {
            mouseOverRowIndex = null;
        }

        Widgets.EndScrollView();

        Text.Anchor = prevTextAnchor;
    }
    void DrawHeaders(Rect targetRect, List<ColumnDef> columns, Vector2? scrollPosition = null)
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = AdjustLastColumnWidth(targetRect, new Rect(currX, 0, column.minWidth, targetRect.height), columns, column);
            column.DrawHeaderCell(cellRect);

            currX += column.minWidth;
        }

        Widgets.EndGroup();
    }
    void DrawRows(Rect targetRect, List<ColumnDef> columns, Vector2 scrollPosition)
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;
        int debug_columnsDrawn = 0;

        // Rows
        for (int i = 0; i < rows.Count; i++)
        {
            // Culling
            if (currY + rowHeight <= 0)
            {
                currY += rowHeight;
                continue;
            }
            else if (currY >= targetRect.height)
            {
                break;
            }

            debug_columnsDrawn = 0;

            var row = rows[i];
            float currX = -scrollPosition.x;

            // Cells
            foreach (var column in columns)
            {
                // Culling
                if (currX + column.minWidth <= 0)
                {
                    currX += column.minWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = AdjustLastColumnWidth(targetRect, new Rect(currX, currY, column.minWidth, rowHeight), columns, column);
                column.DrawCell(cellRect, row);

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            var rowRect = new Rect(0, currY, currX, rowHeight);

            if (Mouse.IsOver(rowRect))
            {
                mouseOverRowIndex = i;
            }

            if (mouseOverRowIndex == i)
            {
                Widgets.DrawHighlight(rowRect);
            }
            else if (i % 2 == 0)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            currY += rowHeight;
            debug_rowsDrawn++;
        }

        Widgets.Label(new Rect(targetRect.width / 2, targetRect.height / 2, 300f, 30f), debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    Rect AdjustLastColumnWidth(Rect parentRect, Rect targetRect, List<ColumnDef> columns, ColumnDef column)
    {
        if (column == columns[columns.Count - 1] && targetRect.xMax < parentRect.width)
        {
            return new Rect(targetRect.x, targetRect.y, parentRect.width - targetRect.x, targetRect.height);
        }

        return targetRect;
    }
}

class ColumnDef
{
    public readonly string? label;
    public readonly string? description;
    public readonly float minWidth = 100f;
    public readonly bool isPinned;
    public readonly List<string> categories;
    public ColumnDef(
        List<string> categories,
        string? label = null,
        string? description = null,
        float? minWidth = null,
        bool isPinned = false
    )
    {
        this.label = label;
        this.description = description;
        if (minWidth is float _minWidth) this.minWidth = _minWidth;
        this.isPinned = isPinned;
        this.categories = categories;
    }
    public virtual void DrawCell(Rect targetRect, Row row)
    {
        // Not very performant, because border will be rendered for each individual cell.
        Utils.DrawLineVertical(targetRect.x + targetRect.width, targetRect.y, Table.rowHeight, Table.columnSeparatorLineColor);
    }
    public virtual void DrawHeaderCell(Rect targetRect)
    {
        if (label != null)
        {
            Widgets.DrawHighlight(targetRect);
            Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), label);
        }

        Utils.DrawLineVertical(targetRect.xMax, targetRect.y, targetRect.height, Table.columnSeparatorLineColor);

        if (Mouse.IsOver(targetRect) && description != null)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(description));
        }
    }
}

class StatColumnDef : ColumnDef
{
    public readonly StatDef statDef;
    public StatColumnDef(
        string statDefName,
        List<string> categories,
        string? label = null,
        float? minWidth = null,
        bool isPinned = false
    ) : base(
        categories,
        label ?? StatDef.Named(statDefName).LabelCap,
        StatDef.Named(statDefName).description,
        minWidth,
        isPinned
    )
    {
        statDef = StatDef.Named(statDefName);
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var cell = row.GetCell(statDef);

        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), cell.valueDisplay ?? "");

        if (Mouse.IsOver(targetRect) && !string.IsNullOrEmpty(cell.valueExplanation) && Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cell.valueExplanation));
        }
    }
}

class LabelColumnDef : ColumnDef
{
    public LabelColumnDef(
        List<string> categories,
        string? label = "Name",
        string? description = null,
        float minWidth = 250f,
        bool isPinned = true
    ) : base(
        categories,
        label,
        description,
        minWidth,
        isPinned
    )
    {
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        var iconRect = new Rect(contentRect.x, contentRect.y, contentRect.height, contentRect.height);
        Widgets.DefIcon(iconRect, row.thingDef);

        var textRect = new Rect(iconRect.xMax + Table.cellPaddingHor, contentRect.y, contentRect.width - iconRect.width - Table.cellPaddingHor, contentRect.height);
        Widgets.LabelEllipses(textRect, row.thingDef.LabelCap == null ? row.thingDef.ToString() : row.thingDef.LabelCap);

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
    //public override void DrawHeaderCell(Rect targetRect)
    //{
    //}
}

class Row
{
    public readonly ThingDef thingDef;
    private readonly Dictionary<string, Cell> cells = [];
    public Row(ThingDef thingDef)
    {
        this.thingDef = thingDef;
    }
    public Cell GetCell(StatDef statDef)
    {
        cells.TryGetValue(statDef.defName, out Cell cell);

        if (cell == null)
        {
            return cells[statDef.defName] = new Cell(thingDef, statDef);
        }
        else
        {
            return cells[statDef.defName];
        }
    }
}

class Cell
{
    public readonly float valueRaw;
    public readonly string valueDisplay;
    public readonly string valueExplanation;
    public Cell(ThingDef thingDef, StatDef statDef)
    {
        valueRaw = thingDef.GetStatValueAbstract(statDef);
        var statReq = StatRequest.For(thingDef, thingDef.defaultStuff);
        // Why valueRaw as final value?
        valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, valueRaw);

        // This is very expensive.
        try
        {
            // Why ToStringNumberSense.Absolute?
            valueDisplay = statDef.Worker.GetStatDrawEntryLabel(statDef, valueRaw, ToStringNumberSense.Absolute, statReq);
        }
        catch
        {

        }
    }
}