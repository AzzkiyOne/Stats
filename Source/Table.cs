using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

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
    public readonly List<Column> columns = [];
    public readonly List<Column> pinnedColumns = [];
    readonly List<Row> rows;
    readonly float columnsWidth = 0f;
    readonly float pinnedColumnsWidth = 0f;
    readonly float minRowWidth = 0f;
    readonly float totalRowsHeight = 0f;
    int? mouseOverRowIndex = null;
    public Table(List<Column> columns, List<Row> rows)
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
    void DrawHeaders(Rect targetRect, List<Column> columns, Vector2? scrollPosition = null)
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
    void DrawRows(Rect targetRect, List<Column> columns, Vector2 scrollPosition)
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int renderedRowsCount = 0;
        int renderedColumnsCount = 0;

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

            renderedColumnsCount = 0;

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
                renderedColumnsCount++;
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
            renderedRowsCount++;
        }

        Widgets.Label(new Rect(targetRect.width / 2, targetRect.height / 2, 300f, 30f), renderedRowsCount + "/" + renderedColumnsCount);

        Widgets.EndGroup();
    }
    Rect AdjustLastColumnWidth(Rect parentRect, Rect targetRect, List<Column> columns, Column column)
    {
        if (column == columns[columns.Count - 1] && targetRect.xMax < parentRect.width)
        {
            return new Rect(targetRect.x, targetRect.y, parentRect.width - targetRect.x, targetRect.height);
        }

        return targetRect;
    }
}

class Column
{
    public readonly string? label;
    public readonly string? description;
    public readonly float minWidth = 100f;
    public readonly bool isPinned;
    public Column(string? label = null, string? description = null, float? minWidth = null, bool isPinned = false)
    {
        this.label = label;
        this.description = description;
        if (minWidth is float _minWidth) this.minWidth = _minWidth;
        this.isPinned = isPinned;
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

class StatColumn : Column
{
    public readonly string key;
    public StatColumn(string key, string? label = null, string? description = null, float? minWidth = null, bool isPinned = false) : base(label, description, minWidth, isPinned)
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
        if (Mouse.IsOver(targetRect) && !string.IsNullOrEmpty(cellExplanation) && Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cellExplanation));
        }
    }
}

class LabelColumn : Column
{
    public LabelColumn(string? label = "Name", string? description = null, float minWidth = 250f, bool isPinned = true) : base(label, description, minWidth, isPinned)
    {
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        var iconRect = new Rect(contentRect.x, contentRect.y, contentRect.height, contentRect.height);
        Widgets.DefIcon(iconRect, row.def);

        var textRect = new Rect(iconRect.xMax + Table.cellPaddingHor, contentRect.y, contentRect.width - iconRect.width - Table.cellPaddingHor, contentRect.height);
        Widgets.LabelEllipses(textRect, row.def.LabelCap == null ? row.def.ToString() : row.def.LabelCap);

        if (Mouse.IsOver(targetRect))
        {
            Widgets.DrawHighlight(targetRect);

            if (Event.current.control)
            {
                TooltipHandler.TipRegion(targetRect, new TipSignal(row.def.LabelCap + "\n\n" + row.def.description));
            }
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
