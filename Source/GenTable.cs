﻿global using GenTableCellDrawData = (
    string label,
    string tip,
    Verse.Def? def,
    Verse.ThingDef? stuff
);

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

internal class GenTable<ColumnType, RowType>
    where ColumnType : class, IGenTableColumn<RowType>
    where RowType : class
{
    private Vector2 scrollPosition = new();
    public List<ColumnType> Columns
    {
        set
        {
            pinnedLeftColumns.Clear();
            middleColumns.Clear();
            pinnedLeftColumnsWidth = 0;
            //middleColumnsWidth = 0;
            minRowWidth = 0;

            if (SortColumn is null && value.First() != null)
            {
                SortColumn = value.First();
            }

            foreach (var column in value)
            {
                if (column == value.First())
                {
                    pinnedLeftColumns.Add(column);
                    pinnedLeftColumnsWidth += column.MinWidth;
                }
                else
                {
                    middleColumns.Add(column);
                    //middleColumnsWidth += column.minWidth;
                }

                minRowWidth += column.MinWidth;
            }
        }
    }
    private readonly List<ColumnType> middleColumns = [];
    private readonly List<ColumnType> pinnedLeftColumns = [];
    private List<RowType> _rows = [];
    public List<RowType> Rows
    {
        get
        {
            return _rows;
        }
        set
        {
            _rows = value;
            totalRowsHeight = value.Count * rowHeight;

            SortRows();
        }
    }
    //private float middleColumnsWidth = 0f;
    private float pinnedLeftColumnsWidth = 0f;
    private float minRowWidth = 0f;
    private float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private ColumnType? _sortColumn;
    private ColumnType? SortColumn
    {
        get
        {
            return _sortColumn;
        }
        set
        {
            if (value == SortColumn)
            {
                sortDirection = (SortDirection)((int)sortDirection * -1);
            }
            else
            {
                //sortDirection = SortDirection.Ascending;
                _sortColumn = value;
            }

            SortRows();
        }
    }
    private SortDirection sortDirection = SortDirection.Ascending;
    public RowType? SelectedRow { get; private set; } = null;

    private const float rowHeight = 30f;
    private const float headersRowHeight = rowHeight;

    //private static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);
    private static Dictionary<int, Color> compareColorMap = new()
    {
        [1] = Color.red,
        [-1] = Color.green,
        [0] = Color.yellow
    };

    public GenTable(List<ColumnType> columns, List<RowType> rows)
    {
        Columns = columns;
        Rows = rows;
    }

    // There might be an issue where scroll area is smaller than total columns width.
    // Probably fixed, but should check.
    public void Draw(Rect targetRect)
    {
        using (new GameFontCtx(GameFont.Small))
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            var contentRect = new Rect(
                0f,
                0f,
                minRowWidth,
                totalRowsHeight + headersRowHeight
            );

            if (
                Event.current.isScrollWheel
                && Event.current.control
                && Mouse.IsOver(targetRect)
            )
            {
                var scrollAmount = Event.current.delta.y * 10;
                var newScrollX = scrollPosition.x + scrollAmount;

                if (newScrollX >= 0)
                {
                    scrollPosition.x = newScrollX;
                }
                else
                {
                    scrollPosition.x = 0;
                }

                Event.current.Use();
            }

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

            var headersRect = new Rect(
                scrollPosition.x,
                scrollPosition.y,
                targetRect.width,
                headersRowHeight
            );
            DrawHeaders(headersRect);

            var bodyRect = new Rect(
                scrollPosition.x,
                scrollPosition.y + headersRowHeight,
                targetRect.width,
                targetRect.height - headersRowHeight
            );
            DrawBody(bodyRect);

            // Separators
            GUIWidgets.DrawLineVertical(
                scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                StatsMainTabWindow.borderLineColor
            );
            Widgets.DrawLineHorizontal(
                scrollPosition.x,
                headersRowHeight + scrollPosition.y,
                targetRect.width,
                StatsMainTabWindow.borderLineColor
            );
            GUIWidgets.DrawLineVertical(
                pinnedLeftColumnsWidth + scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                StatsMainTabWindow.borderLineColor
            );

            if (!Mouse.IsOver(bodyRect))
            {
                mouseOverRowIndex = null;
            }

            Widgets.EndScrollView();
        }
    }
    private void DrawHeaders(Rect targetRect)
    {
        var currX = targetRect.x;

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, pinnedLeftColumnsWidth),
            pinnedLeftColumns
        );
        // Draw middle headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition
        );
    }
    private void DrawHeaderColumns(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2? scrollPosition = null
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = new Rect(currX, 0, column.MinWidth, targetRect.height);

            AdjustColumnWidthIfLastColumn(targetRect, ref cellRect, columns, column);

            if (DrawHeaderCell(cellRect, column))
            {
                SortColumn = column;
            }

            currX += column.MinWidth;
        }

        Widgets.EndGroup();
    }
    private bool DrawHeaderCell(Rect targetRect, ColumnType column)
    {
        //Widgets.DrawHighlight(targetRect);
        Widgets.Label(targetRect.ContractedBy(GenUI.Pad, 0), column.Label);

        if (SortColumn == column)
        {
            var rotationAngle = (int)sortDirection * -90f;

            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(targetRect.height),
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

        TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        return Widgets.ButtonInvisible(targetRect);
    }
    private void DrawBody(Rect targetRect)
    {
        var currX = targetRect.x;

        // Draw pinned rows
        DrawRows(
            targetRect.CutFromX(ref currX, pinnedLeftColumnsWidth),
            pinnedLeftColumns,
            new Vector2(0, scrollPosition.y)
        );
        // Draw middle rows
        DrawRows(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition
        );
    }
    private void DrawRows(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2 scrollPosition
    )
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;
        int debug_columnsDrawn = 0;

        // Rows
        for (int i = 0; i < Rows.Count; i++)
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

            var row = Rows[i];
            var isEven = i % 2 == 0;
            var isMouseOver = mouseOverRowIndex == i;
            var rowRect = new Rect(0, currY, targetRect.width, rowHeight);
            float currX = -scrollPosition.x;

            if (isEven && !isMouseOver)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            // Cells
            foreach (var column in columns)
            {
                // Culling
                if (currX + column.MinWidth <= 0)
                {
                    currX += column.MinWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = new Rect(currX, currY, column.MinWidth, rowHeight);

                AdjustColumnWidthIfLastColumn(targetRect, ref cellRect, columns, column);

                DrawRowCell(cellRect, column, row);

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            if (Mouse.IsOver(rowRect))
            {
                mouseOverRowIndex = i;
            }

            if (Widgets.ButtonInvisible(rowRect))
            {
                if (SelectedRow == row)
                {
                    SelectedRow = null;
                }
                else
                {
                    SelectedRow = row;
                }
            }

            if (SelectedRow == row)
            {
                Widgets.DrawHighlightSelected(rowRect);
            }

            if (isMouseOver)
            {
                Widgets.DrawHighlight(rowRect);
            }

            currY += rowHeight;
            debug_rowsDrawn++;
        }

        Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    private void DrawRowCell(Rect targetRect, ColumnType column, RowType row)
    {
        var (label, tip, def, stuff) = column.GetCellDrawData(row);

        if (label == "")
        {
            return;
        }

        if (
            column.IsComparable
            && SelectedRow is not null
            && row != SelectedRow
        )
        {
            var compareResult = column.CompareRows(SelectedRow, row);

            GUI.color = compareColorMap[compareResult];
        }

        var contentRect = targetRect.ContractedBy(GenUI.Pad, 0);
        var currX = contentRect.x;

        if (def is not null)
        {
            // This is very expensive.
            Widgets.DefIcon(contentRect.CutFromX(ref currX, contentRect.height), def, stuff);

            currX += GenUI.Pad;

            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                GUIWidgets.DefInfoDialog(def, stuff);
            }
        }

        Widgets.Label(contentRect.CutFromX(ref currX), label);

        GUI.color = Color.white;

        if (
            //Event.current.control &&
            !string.IsNullOrEmpty(tip)
        )
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(tip));
        }
    }
    // Maybe it could be done once for a whole column.
    private void AdjustColumnWidthIfLastColumn(
        Rect parentRect,
        ref Rect targetRect,
        List<ColumnType> columns,
        ColumnType column
    )
    {
        if (
            column == columns[columns.Count - 1]
            && targetRect.xMax < parentRect.width
        )
        {
            targetRect.xMax = parentRect.width;
        }
    }
    private void SortRows()
    {
        if (SortColumn is null)
        {
            return;
        }

        Rows.Sort((r1, r2) => SortColumn.CompareRows(r1, r2) * (int)sortDirection);
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}

public interface IGenTableColumn<RowType>
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public bool IsComparable { get; }
    public GenTableCellDrawData GetCellDrawData(RowType row);
    public int CompareRows(RowType r1, RowType r2);
}