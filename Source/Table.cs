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
    public const float minColumnWidth = 100f;
    public static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);
    public Vector2 scrollPosition = new();
    public readonly List<Column> columns = new();
    public readonly List<Column> pinnedColumns = new();
    readonly List<Row> rows;
    readonly float columnsWidth = 0f;
    readonly float pinnedColumnsWidth = 0f;
    readonly float totalColumnsWidth = 0f;
    readonly float totalRowsHeight = 0f;
    public Table(List<Column> columns, List<Row> rows)
    {
        this.rows = rows;

        foreach (var column in columns)
        {
            if (column.isPinned)
            {
                pinnedColumns.Add(column);
                pinnedColumnsWidth += column.width;
            }
            else
            {
                this.columns.Add(column);
                columnsWidth += column.width;
            }

            totalColumnsWidth += column.width;
        }

        totalRowsHeight = rowHeight * rows.Count;
    }
    public void Draw(Rect targetRect)
    {
        Text.Font = GameFont.Small;
        TextAnchor prevTextAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;

        var contentRect = new Rect(targetRect.x, targetRect.y, totalColumnsWidth, totalRowsHeight + headersRowHeight);

        Utils.DrawLineVertical(pinnedColumnsWidth, 0f, targetRect.height, new(1f, 1f, 1f, 0.4f));
        Widgets.DrawLineHorizontal(0f, headersRowHeight, targetRect.width, new(1f, 1f, 1f, 0.4f));

        Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

        // Draw pinned headers
        var pinnedHeadersRowRect = new Rect(scrollPosition.x, scrollPosition.y, pinnedColumnsWidth, headersRowHeight);
        DrawHeaders(pinnedHeadersRowRect, pinnedColumns);

        // Draw headers
        var headersRowRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y, contentRect.xMax - pinnedColumnsWidth, rowHeight);
        DrawHeaders(headersRowRect, columns, scrollPosition);

        // Draw pinned rows
        var pinnedTableBodyRect = new Rect(scrollPosition.x, scrollPosition.y + headersRowHeight, pinnedColumnsWidth, contentRect.height - headersRowHeight);
        DrawRows(pinnedTableBodyRect, pinnedColumns, new Vector2(0, scrollPosition.y));

        // Draw rows
        //var tableBodyRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y + headersRowHeight, totalColumnsWidth, contentRect.height - headersRowHeight);
        var tableBodyRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y + headersRowHeight, targetRect.width - pinnedColumnsWidth, targetRect.height - headersRowHeight);
        DrawRows(tableBodyRect, columns, scrollPosition);

        Widgets.EndScrollView();

        Text.Anchor = prevTextAnchor;
    }
    void DrawHeaders(Rect targetRect, List<Column> columns, Vector2? scrollPosition = null)
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = new Rect(currX, 0, column.width, targetRect.height);
            column.DrawHeaderCell(cellRect);

            currX += column.width;
        }

        Widgets.EndGroup();
    }
    void DrawRows(Rect targetRect, List<Column> columns, Vector2 scrollPosition)
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int renderedRowsCount = 0;

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

            var row = rows[i];
            float currX = -scrollPosition.x;

            // Cells
            foreach (var column in columns)
            {
                // Culling
                //if (scrollPosition.x > currX + column.width)
                //{
                //    currX += column.width;
                //    continue;
                //}
                //else if (currX > targetRect.width + scrollPosition.x)
                //{
                //    break;
                //}

                var cellRect = new Rect(currX, currY, column.width, rowHeight);
                column.DrawCell(cellRect, row);

                currX += column.width;
            }

            if (i % 2 == 0)
            {
                var rowRect = new Rect(0, currY, totalColumnsWidth, rowHeight);
                Widgets.DrawLightHighlight(rowRect);
            }

            currY += rowHeight;
            renderedRowsCount++;
        }

        Widgets.Label(new Rect(targetRect.width / 2, targetRect.height / 2, 300f, 30f), "Rendered rows: " + renderedRowsCount);

        Widgets.EndGroup();
    }
}

class Column
{
    public readonly string label;
    public readonly string description;
    public readonly float width;
    public readonly bool isPinned;
    public Column(string label = null, string description = null, float width = Table.minColumnWidth, bool isPinned = false)
    {
        this.label = label;
        this.description = description;
        this.width = width >= Table.minColumnWidth ? width : Table.minColumnWidth;
        this.isPinned = isPinned;
    }
    public virtual void DrawCell(Rect targetRect, Row row)
    {
        // Not very performant, because border will be rendered for each individual cell.
        Utils.DrawLineVertical(targetRect.x + targetRect.width, targetRect.y, Table.rowHeight, Table.columnSeparatorLineColor);
    }
    public virtual void DrawHeaderCell(Rect targetRect)
    {
        Widgets.DrawHighlight(targetRect);
        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), label);
        Utils.DrawLineVertical(targetRect.xMax, targetRect.y, targetRect.height, Table.columnSeparatorLineColor);

        if (Mouse.IsOver(targetRect) && description != null)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(description));
        }
    }
}

class StatColumn : Column
{
    public readonly string key;
    public StatColumn(string key, string label = null, string description = null, float width = 100f, bool isPinned = false) : base(label, description, width, isPinned)
    {
        this.key = key;
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        row.stats.TryGetValue(key, out var statDrawEntry);
        if (statDrawEntry == null)
        {
            return;
        }

        var cellValue = statDrawEntry.ValueString;
        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), cellValue);

        var cellExplanation = statDrawEntry.GetExplanationText(statDrawEntry.optionalReq);
        // This is dirty, but it does the job.
        if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(cellExplanation))
        {
            cellExplanation = cellExplanation.Replace(description, "").TrimStart();
        }
        if (Mouse.IsOver(targetRect) && !string.IsNullOrEmpty(cellExplanation))
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cellExplanation));
        }
    }
}

class LabelColumn : Column
{
    public LabelColumn(string label = null, string description = null, float width = 250f, bool isPinned = true) : base(label, description, width, isPinned)
    {
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        var iconRect = new Rect(contentRect.x, contentRect.y, contentRect.height, contentRect.height);
        Widgets.DefIcon(iconRect, row.def);

        var textRect = new Rect(iconRect.xMax + Table.cellPaddingHor, contentRect.y, contentRect.width - iconRect.width - Table.cellPaddingHor, contentRect.height);
        Widgets.LabelEllipses(textRect, row.def.LabelCap);

        if (Mouse.IsOver(targetRect))
        {
            Widgets.DrawHighlight(targetRect);

            TooltipHandler.TipRegion(targetRect, new TipSignal(row.def.LabelCap + "\n\n" + row.def.description));
        }

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(row.def));
        }
    }
    //public override void DrawHeaderCell(Rect targetRect)
    //{
    //}
}

class Row
{
    public readonly ThingDef def;
    public readonly Dictionary<string, StatDrawEntry> stats = [];
    public Row(ThingDef def)
    {
        this.def = def;

        foreach (var stat in Utils.GetAllDefDisplayStats(def, def.defaultStuff))
        {
            if (!stats.ContainsKey(stat.LabelCap))
            {
                stats[stat.LabelCap] = stat;
            }
        }
    }
}
