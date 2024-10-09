using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class TableWidget
{
    private Vector2 ScrollPosition = new();
    private readonly List<HeaderCellWidget> HeaderCells;
    private int? MouseOverRowIndex = null;
    private ColumnDef? _sortColumn;
    private ColumnDef? SortColumn
    {
        get => _sortColumn;
        set
        {
            if (value == SortColumn)
            {
                SortDirection = (SortDirection)((int)SortDirection * -1);
            }
            else
            {
                //sortDirection = SortDirection.Ascending;
                _sortColumn = value;
            }

            SortRows();
        }
    }
    private SortDirection SortDirection = SortDirection.Ascending;
    private Dictionary<ColumnDef, ICellWidget?>? _selectedRow = null;
    private Dictionary<ColumnDef, ICellWidget?>? SelectedRow
    {
        get => _selectedRow;
        set
        {
            if (value == _selectedRow)
            {
                value = null;
            }

            _selectedRow = value;

            if (_selectedRow == null)
            {
                foreach (var row in Rows)
                {
                    foreach (var headerCell in HeaderCells)
                    {
                        var cell = row.TryGetValue(headerCell.Column);

                        if (cell is CellWidget_Num numCell)
                        {
                            numCell.Reset();
                        }
                    }
                }
            }
            else
            {
                foreach (var row in Rows)
                {
                    foreach (var headerCell in HeaderCells)
                    {
                        var cell = row.TryGetValue(headerCell.Column);
                        var selRowCell = _selectedRow.TryGetValue(headerCell.Column);

                        if (cell is CellWidget_Num numCell)
                        {
                            if (row == _selectedRow || selRowCell == null)
                            {
                                numCell.Reset();
                            }
                            else if (selRowCell is CellWidget_Num otherCell)
                            {
                                numCell.Color = (numCell.CompareTo(otherCell) * (headerCell.Column.BestIsHighest ? 1 : -1)) switch
                                {
                                    < 0 => Color.red,
                                    > 0 => Color.green,
                                    0 => Color.yellow,
                                };
                            }
                        }
                    }
                }
            }
        }
    }
    public const float RowHeight = 30f;
    private const float HeadersRowHeight = RowHeight;
    public const float CellPadding = 10f;
    public const float CellMinWidth = CellPadding * 2f;
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private List<Dictionary<ColumnDef, ICellWidget?>> _rows = [];
    public List<Dictionary<ColumnDef, ICellWidget?>> Rows
    {
        get => _rows;
        set
        {
            _rows = value;
            // This can cause double sorting.
            SortRows();
        }
    }
    private readonly TablePart Left;
    private readonly TablePart Right;
    public TableWidget(List<ThingAlike> things, List<ColumnDef> columns)
    {
        HeaderCells = new(columns.Count);

        var leftHeaderCells = new List<HeaderCellWidget>();
        var rightHeaderCells = new List<HeaderCellWidget>();

        foreach (var column in columns)
        {
            var headerCell = new HeaderCellWidget(column);

            HeaderCells.Add(headerCell);

            if (column == ColumnDefOf.Id)
            {
                leftHeaderCells.Add(headerCell);
            }
            else
            {
                rightHeaderCells.Add(headerCell);
            }
        }

        if (leftHeaderCells.First() != null)
        {
            SortColumn = leftHeaderCells.First().Column;
        }

        var rows = new List<Dictionary<ColumnDef, ICellWidget?>>();

        foreach (var thing in things)
        {
            var row = new Dictionary<ColumnDef, ICellWidget?>(HeaderCells.Count);

            foreach (var headerCell in HeaderCells)
            {
                try
                {
                    var cell = headerCell.Column.GetCellWidget(thing);

                    if (cell != null)
                    {
                        row.Add(headerCell.Column, cell);

                        headerCell.MinWidth = Math.Max(headerCell.MinWidth, cell.MinWidth);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            rows.Add(row);
        }

        Rows = rows;
        Left = new(this, leftHeaderCells);
        Right = new(this, rightHeaderCells);
    }
    public void Draw(Rect targetRect)
    {
        if (
            Event.current.isScrollWheel
            && Event.current.control
            && Mouse.IsOver(targetRect)
        )
        {
            var scrollAmount = Event.current.delta.y * 10f;
            var newScrollX = ScrollPosition.x + scrollAmount;

            ScrollPosition.x = newScrollX >= 0f ? newScrollX : 0f;
            Event.current.Use();
        }

        var contentSizeMax = new Vector2(
            Left.MinWidth + Right.MinWidth,
            Rows.Count * RowHeight + HeadersRowHeight
        );
        var willScrollHor = contentSizeMax.x > targetRect.width;
        var willScrollVer = contentSizeMax.y > targetRect.height;
        var contentSizeVisible = new Vector2(
            willScrollVer ? targetRect.width - GenUI.ScrollBarWidth : targetRect.width,
            willScrollHor ? targetRect.height - GenUI.ScrollBarWidth : targetRect.height
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );

        Widgets.BeginScrollView(targetRect, ref ScrollPosition, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPosition, contentSizeVisible);

        Widgets.DrawHighlight(contentRectVisible.TopPartPixels(HeadersRowHeight));

        var curX = contentRectVisible.x;
        var leftPartRect = contentRectVisible.CutFromX(ref curX, Left.MinWidth);
        var signalsLeft = Left.Draw(leftPartRect, new Vector2(0f, ScrollPosition.y));

        LineVerticalWidget.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        var rightPartRect = contentRectVisible.CutFromX(ref curX);
        var signalsRight = Right.Draw(rightPartRect, ScrollPosition);

        Widgets.DrawLineHorizontal(
            contentRectVisible.x,
            contentRectVisible.y + HeadersRowHeight,
            contentRectVisible.width,
            StatsMainTabWindow.BorderLineColor
        );

        Widgets.EndScrollView();

        var clickedRowIndex = signalsLeft.clickedRowIndex ?? signalsRight.clickedRowIndex;
        var clickedHeaderCell = signalsLeft.clickedHeaderCell ?? signalsRight.clickedHeaderCell;

        MouseOverRowIndex = signalsLeft.mouseOverRowIndex ?? signalsRight.mouseOverRowIndex;

        if (clickedRowIndex is int rowIndex)
        {
            SelectedRow = Rows[rowIndex];
        }

        if (clickedHeaderCell != null)
        {
            SortColumn = clickedHeaderCell.Column;
        }
    }
    private void SortRows()
    {
        if (SortColumn == null)
        {
            return;
        }

        Rows.Sort((r1, r2) =>
        {
            var r1c = r1.TryGetValue(SortColumn);
            var r2c = r2.TryGetValue(SortColumn);

            if (r1c == null && r2c == null)
            {
                return 0;
            }

            return (r1c?.CompareTo(r2c) ?? -1) * (int)SortDirection;
        });
    }
    private sealed class TablePart
    {
        private readonly TableWidget Parent;
        private readonly List<HeaderCellWidget> HeaderCells;
        public float MinWidth { get; } = 0f;
        private float PrevFrameScrollPosX = 0f;
        private float PrevFrameTargetRectWidth = 0f;
        private float CurFrameCellOffsetX = 0f;
        private readonly List<HeaderCellWidget> CurFrameHeaderCells;
        public TablePart(TableWidget parent, List<HeaderCellWidget> headerCells)
        {
            Parent = parent;
            HeaderCells = headerCells;
            CurFrameHeaderCells = new(headerCells.Count);

            foreach (var headerCell in headerCells)
            {
                MinWidth += headerCell.MinWidth;
            }
        }
        public (
            int? mouseOverRowIndex,
            int? clickedRowIndex,
            HeaderCellWidget? clickedHeaderCell
        ) Draw(Rect targetRect, Vector2 scrollPosition)
        {
            var cellExtraWidth = Math.Max((targetRect.width - MinWidth) / HeaderCells.Count, 0f);

            if (
                PrevFrameScrollPosX != scrollPosition.x
                || PrevFrameTargetRectWidth != targetRect.width
            )
            {
                UpdateCurFrameState(targetRect, scrollPosition, cellExtraWidth);
            }

            var curY = targetRect.y;

            var clickedHeaderCell = DrawHeaders(
                targetRect.CutFromY(ref curY, HeadersRowHeight),
                cellExtraWidth
            );
            var (mouseOverRowIndex, clickedRowIndex) = DrawBody(
                targetRect.CutFromY(ref curY),
                scrollPosition.y,
                cellExtraWidth
            );
            DrawColumnSeparators(targetRect, cellExtraWidth);

            return (mouseOverRowIndex, clickedRowIndex, clickedHeaderCell);
        }
        private HeaderCellWidget? DrawHeaders(Rect targetRect, float cellExtraWidth)
        {
            Widgets.BeginGroup(targetRect);

            var curX = CurFrameCellOffsetX;
            HeaderCellWidget? clickedCell = null;

            foreach (var cell in CurFrameHeaderCells)
            {
                var cellRect = new Rect(
                    curX,
                    0f,
                    cell.MinWidth + cellExtraWidth,
                    targetRect.height
                );

                curX = cellRect.xMax;

                if (cell.Draw(cellRect, Parent.SortColumn, Parent.SortDirection))
                {
                    clickedCell = cell;
                }
            }

            Widgets.EndGroup();

            return clickedCell;
        }
        private (
            int? mouseOverRowIndex,
            int? clickedRowIndex
        ) DrawBody(
            Rect targetRect,
            float scrollPositionY,
            float cellExtraWidth
        )
        {
            Widgets.BeginGroup(targetRect);

            // For some reason returning a row object makes performance around 2 times worse.
            int? clickedRowIndex = null;
            int? mouseOverRowIndex = null;
            var rowIndexStart = (int)Math.Floor(scrollPositionY / RowHeight);
            var rowIndexEnd = Math.Min(
                (int)Math.Ceiling((scrollPositionY + targetRect.height) / RowHeight),
                Parent.Rows.Count
            );

            // Rows
            for (int rowIndex = rowIndexStart; rowIndex < rowIndexEnd; rowIndex++)
            {
                var curY = rowIndex * RowHeight - scrollPositionY;
                var rowRect = new Rect(0f, curY, targetRect.width, RowHeight);
                var isMouseOver = Parent.MouseOverRowIndex == rowIndex;

                if (rowIndex % 2 == 0 && !isMouseOver)
                {
                    Widgets.DrawLightHighlight(rowRect);
                }

                var row = Parent.Rows[rowIndex];
                var curX = CurFrameCellOffsetX;

                // Cells
                foreach (var headerCell in CurFrameHeaderCells)
                {
                    var cell = row.TryGetValue(headerCell.Column);
                    var cellWidth = headerCell.MinWidth + cellExtraWidth;
                    // We do not create a separate variable for cell's rect because it is
                    // created conditionally (cell?.Draw).
                    cell?.Draw(new Rect(curX, curY, cellWidth, RowHeight));

                    curX += cellWidth;
                }

                if (Mouse.IsOver(rowRect))
                {
                    mouseOverRowIndex = rowIndex;
                }

                if (Widgets.ButtonInvisible(rowRect))
                {
                    clickedRowIndex = rowIndex;
                }

                if (Parent.SelectedRow == row)
                {
                    Widgets.DrawHighlightSelected(rowRect);
                }

                if (isMouseOver)
                {
                    Widgets.DrawHighlight(rowRect);
                }
            }

            //Debug.TryDrawUIDebugInfo(targetRect, rowIndexEnd - rowIndexStart + "/" + CurFrameHeaderCells.Count);

            Widgets.EndGroup();

            return (mouseOverRowIndex, clickedRowIndex);
        }
        private void DrawColumnSeparators(Rect targetRect, float cellExtraWidth)
        {
            var curSepX = CurFrameCellOffsetX + targetRect.x;
            // Separators
            for (int i = 0; i < CurFrameHeaderCells.Count - 1; i++)
            {
                curSepX += CurFrameHeaderCells[i].MinWidth + cellExtraWidth;

                LineVerticalWidget.Draw(
                    curSepX,
                    targetRect.y,
                    targetRect.height,
                    ColumnSeparatorLineColor
                );
            }
        }
        private void UpdateCurFrameState(
            Rect targetRect,
            Vector2 scrollPosition,
            float cellExtraWidth
        )
        {
            PrevFrameScrollPosX = scrollPosition.x;
            PrevFrameTargetRectWidth = targetRect.width;
            CurFrameCellOffsetX = -scrollPosition.x;
            CurFrameHeaderCells.Clear();

            var curX = CurFrameCellOffsetX;

            for (int i = 0; i < HeaderCells.Count && curX < targetRect.width; i++)
            {
                var headerCell = HeaderCells[i];
                var cellWidth = headerCell.MinWidth + cellExtraWidth;

                if (curX + cellWidth <= 0f)
                {
                    curX += cellWidth;
                    CurFrameCellOffsetX = curX;
                    continue;
                }

                curX += cellWidth;
                CurFrameHeaderCells.Add(headerCell);
            }
        }
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
