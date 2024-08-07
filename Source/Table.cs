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
    public readonly List<ColumnDef> middleColumns = [];
    public readonly List<ColumnDef> pinnedColumns = [];
    private readonly List<Row> rows;
    private readonly float middleColumnsWidth = 0f;
    private readonly float pinnedColumnsWidth = 0f;
    private readonly float minRowWidth = 0f;
    private readonly float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private ColumnDef? sortColumnDef;
    private SortDirection sortDirection = SortDirection.Ascending;
    private bool dragInProgress = false;
    public Table(List<ColumnDef> columns, List<Row> rows)
    {
        this.rows = rows;

        if (columns[0] != null)
        {
            sortColumnDef = columns[0];
            sortColumnDef.SortRows(rows, sortDirection);
        }

        foreach (var column in columns)
        {
            if (column.isPinned)
            {
                pinnedColumns.Add(column);
                pinnedColumnsWidth += column.minWidth;
            }
            else
            {
                middleColumns.Add(column);
                middleColumnsWidth += column.minWidth;
            }

            minRowWidth += column.minWidth;
        }

        totalRowsHeight = rowHeight * rows.Count;
    }
    public void Draw(Rect targetRect)
    {
        using (new GUIUtils.GameFontContext(GameFont.Small))
        using (new GUIUtils.TextAnchorContext(TextAnchor.MiddleLeft))
        {
            var contentRect = new Rect(0f, 0f, minRowWidth, totalRowsHeight + headersRowHeight);

            // If the table stops drawing while the user is dragging the mouse on the screen (for example the table is rendered in a window and the user pressed ESC)
            // we end up in a state where dragInProgess == true. I'm not sure if the code below is a good way to fix this, but it looks harmless.
            if (GUIUtility.hotControl == 0)
            {
                dragInProgress = false;
            }

            var id = GUIUtility.GetControlID(FocusType.Passive);

            if (dragInProgress)
            {
                // Taking control over GUI events while dragging is in progress.
                GUIUtility.hotControl = id;

                // Catching "mouse up" event to stop drag.
                if (Event.current.GetTypeForControl(id) == EventType.MouseUp)
                {
                    GUIUtility.hotControl = 0;
                }
            }

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

            // Draw pinned headers
            var pinnedHeadersRowRect = new Rect(scrollPosition.x, scrollPosition.y, pinnedColumnsWidth, headersRowHeight);
            DrawHeaders(pinnedHeadersRowRect, pinnedColumns);

            // Draw headers
            var headersRowRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y, contentRect.xMax - pinnedColumnsWidth, rowHeight);
            DrawHeaders(headersRowRect, middleColumns, scrollPosition);

            // Draw pinned rows
            var pinnedTableBodyRect = new Rect(scrollPosition.x, scrollPosition.y + headersRowHeight, pinnedColumnsWidth, targetRect.height - headersRowHeight);
            DrawRows(pinnedTableBodyRect, pinnedColumns, new Vector2(0, scrollPosition.y));

            // Draw rows
            var tableBodyRect = new Rect(scrollPosition.x + pinnedColumnsWidth, scrollPosition.y + headersRowHeight, targetRect.width - pinnedColumnsWidth, targetRect.height - headersRowHeight);
            DrawRows(tableBodyRect, middleColumns, scrollPosition);

            // Separators
            GUIUtils.DrawLineVertical(scrollPosition.x, scrollPosition.y, targetRect.height, new(1f, 1f, 1f, 0.4f));
            Widgets.DrawLineHorizontal(scrollPosition.x, headersRowHeight + scrollPosition.y, targetRect.width, new(1f, 1f, 1f, 0.4f));
            GUIUtils.DrawLineVertical(pinnedColumnsWidth + scrollPosition.x, scrollPosition.y, targetRect.height, new(1f, 1f, 1f, 0.4f));

            // Initiate drag when the user holds left mouse button down in the (not always) scrollable table area.
            if (
                !dragInProgress
                && Mouse.IsOver(tableBodyRect)
                && Event.current.type == EventType.MouseDown
            )
            {
                dragInProgress = true;
            }

            // Adjust horizontal scroll position on drag event.
            if (
                dragInProgress
                && Event.current.GetTypeForControl(id) == EventType.MouseDrag
            )
            {
                scrollPosition.x = Mathf.Clamp(scrollPosition.x + Event.current.delta.x, 0, contentRect.width - targetRect.width + GUI.skin.verticalScrollbar.fixedWidth);
            }

            if (!Mouse.IsOver(pinnedTableBodyRect.Union(tableBodyRect)))
            {
                mouseOverRowIndex = null;
            }

            Widgets.EndScrollView();
        }
    }
    void DrawHeaders(Rect targetRect, List<ColumnDef> columns, Vector2? scrollPosition = null)
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = AdjustLastColumnWidth(targetRect, new Rect(currX, 0, column.minWidth, targetRect.height), columns, column);
            if (column.DrawHeaderCell(cellRect, sortColumnDef == column ? sortDirection : null))
            {
                HandleHeaderRowCellClick(column);
            }

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

        Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

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
    private void HandleHeaderRowCellClick(ColumnDef columnDef)
    {
        if (columnDef == null)
        {
            return;
        }

        if (sortColumnDef == columnDef)
        {
            if (sortDirection == SortDirection.Ascending)
            {
                sortDirection = SortDirection.Descending;
            }
            else
            {
                sortDirection = SortDirection.Ascending;
            }
        }
        else
        {
            sortColumnDef = columnDef;
            sortDirection = SortDirection.Ascending;
        }

        sortColumnDef.SortRows(rows, sortDirection);
    }
}

enum SortDirection
{
    Ascending,
    Descending,
}

class ColumnDef
{
    public string? label { get; init; }
    public string? description { get; init; }
    public float minWidth { get; init; } = 100f;
    public bool isPinned { get; init; } = false;
    public bool isSortable { get; init; } = true;
    public virtual void DrawCell(Rect targetRect, Row row)
    {
        // Not very performant, because border will be rendered for each individual cell.
        GUIUtils.DrawLineVertical(targetRect.x + targetRect.width, targetRect.y, Table.rowHeight, Table.columnSeparatorLineColor);
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
    }
}

class StatColumnDef : ColumnDef
{
    public StatDef statDef { get; }
    public bool drawRawValue { get; init; } = false;
    public StatColumnDef(string statDefName) : base()
    {
        statDef = StatDef.Named(statDefName);
        label = statDef.LabelCap;
        description = statDef.description;
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var cell = row.GetCell(statDef);

        Widgets.LabelEllipses(targetRect.ContractedBy(Table.cellPaddingHor, 0), Debug.InDebugMode() || drawRawValue ? cell.valueRaw + "" : cell.valueDisplay + "");

        if (Mouse.IsOver(targetRect) && !string.IsNullOrEmpty(cell.valueExplanation) && Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cell.valueExplanation));
        }
    }
    public override void SortRows(List<Row> rows, SortDirection direction)
    {
        rows.Sort((r1, r2) =>
        {
            var val1 = r1.GetCell(statDef).valueRaw;
            var val2 = r2.GetCell(statDef).valueRaw;

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

class LabelColumnDef : ColumnDef
{
    public LabelColumnDef() : base()
    {
        label = "Name";
        minWidth = 250f;
        isPinned = true;
    }
    public override void DrawCell(Rect targetRect, Row row)
    {
        base.DrawCell(targetRect, row);

        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        var iconRect = new Rect(contentRect.x, contentRect.y, contentRect.height, contentRect.height);
        Widgets.DefIcon(iconRect, row.thingDef);

        var textRect = new Rect(iconRect.xMax + Table.cellPaddingHor, contentRect.y, contentRect.width - iconRect.width - Table.cellPaddingHor, contentRect.height);
        string labelText = row.thingDef.LabelCap == null ? row.thingDef.ToString() : row.thingDef.LabelCap;

        if (Debug.InDebugMode())
        {
            labelText = row.thingDef.defName;
        }

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
    //public override void DrawHeaderCell(Rect targetRect)
    //{
    //}
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
    public readonly float? valueRaw;
    public readonly string? valueDisplay;
    public readonly string? valueExplanation;
    public Cell(ThingDef thingDef, StatDef statDef)
    {
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
            var statReq = StatRequest.For(thingDef, thingDef.defaultStuff);

            try
            {
                // Why ToStringNumberSense.Absolute?
                valueDisplay = statDef.Worker.GetStatDrawEntryLabel(statDef, _valueRaw, ToStringNumberSense.Absolute, statReq);
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
    }
}