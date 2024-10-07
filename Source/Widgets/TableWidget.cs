using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class TableWidget
{
    private Vector2 ScrollPosition = new();
    private readonly List<ColumnDef> Columns;
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
                    foreach (var column in Columns)
                    {
                        var cell = row.TryGetValue(column);

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
                    foreach (var column in Columns)
                    {
                        var cell = row.TryGetValue(column);
                        var selRowCell = _selectedRow.TryGetValue(column);

                        if (cell is CellWidget_Num numCell)
                        {
                            if (row == _selectedRow || selRowCell == null)
                            {
                                numCell.Reset();
                            }
                            else if (selRowCell is CellWidget_Num otherCell)
                            {
                                //numCell.Color = (numCell.CompareTo(otherCell) * (column.bestIsHighest ? 1 : -1)) switch
                                //{
                                //    < 0 => Color.red,
                                //    > 0 => Color.green,
                                //    0 => Color.yellow,
                                //};
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
    //private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.1f);
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
    private readonly Dictionary<ColumnDef, float> ColumnMinWidths = [];
    private readonly TablePart Left;
    private readonly TablePart Right;
    public TableWidget(List<ThingAlike> things, List<ColumnDef> columns)
    {
        var leftColumns = new List<ColumnDef>();
        var rightColumns = new List<ColumnDef>();

        foreach (var column in columns)
        {
            if (column == ColumnDefOf.Id)
            {
                leftColumns.Add(column);
            }
            else
            {
                rightColumns.Add(column);
            }

            if (column.Icon != null)
            {
                ColumnMinWidths[column] = RowHeight + CellPadding * 2f;
            }
            else
            {
                ColumnMinWidths[column] = Text.CalcSize(column.Label).x + CellPadding * 2f;
            }
        }

        if (leftColumns.First() != null)
        {
            SortColumn = leftColumns.First();
        }

        var rows = new List<Dictionary<ColumnDef, ICellWidget?>>();

        foreach (var thing in things)
        {
            var row = new Dictionary<ColumnDef, ICellWidget?>(columns.Count);

            foreach (var column in columns)
            {
                try
                {
                    var cell = column.GetCellWidget(thing);

                    if (cell != null)
                    {
                        row.Add(column, cell);

                        ColumnMinWidths[column] = Math.Max(
                            ColumnMinWidths[column],
                            cell.MinWidth + CellPadding * 2f
                        );
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            rows.Add(row);
        }

        Columns = columns;
        Rows = rows;
        Left = new(this, leftColumns);
        Right = new(this, rightColumns);
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
        var contentRectVisible = new Rect(
            ScrollPosition,
            contentSizeVisible
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );
        var curX = contentRectVisible.x;
        var leftPartRect = contentRectVisible.CutFromX(ref curX, Left.MinWidth);
        var rightPartRect = contentRectVisible.CutFromX(ref curX);

        Widgets.BeginScrollView(targetRect, ref ScrollPosition, contentRectMax, true);

        var signalsLeft = Left.Draw(leftPartRect, new Vector2(0f, ScrollPosition.y));

        LineVerticalWidget.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        var signalsRight = Right.Draw(rightPartRect, ScrollPosition);

        Widgets.EndScrollView();

        var clickedRowIndex = signalsLeft.clickedRowIndex ?? signalsRight.clickedRowIndex;
        var clickedColumn = signalsLeft.clickedColumn ?? signalsRight.clickedColumn;

        MouseOverRowIndex = signalsLeft.mouseOverRowIndex ?? signalsRight.mouseOverRowIndex;

        if (clickedRowIndex is int rowIndex)
        {
            SelectedRow = Rows[rowIndex];
        }

        if (clickedColumn != null)
        {
            SortColumn = clickedColumn;
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
        private readonly List<ColumnDef> Columns;
        public float MinWidth { get; } = 0f;
        public TablePart(TableWidget parent, List<ColumnDef> columns)
        {
            Parent = parent;
            Columns = columns;

            foreach (var column in columns)
            {
                MinWidth += Parent.ColumnMinWidths[column];
            }
        }
        public (
            int? mouseOverRowIndex,
            int? clickedRowIndex,
            ColumnDef? clickedColumn
        ) Draw(Rect targetRect, Vector2 scrollPosition)
        {
            var curY = targetRect.y;
            var cellExtraWidth = Math.Max((targetRect.width - MinWidth) / Columns.Count, 0f);

            var (indexStart, offset, indexEnd, clickedColumn) = DrawHeaders(
                targetRect.CutFromY(ref curY, HeadersRowHeight),
                scrollPosition,
                cellExtraWidth
            );
            var (mouseOverRowIndex, clickedRowIndex) = DrawBody(
                targetRect.CutFromY(ref curY),
                scrollPosition,
                cellExtraWidth,
                indexStart,
                offset,
                indexEnd
            );

            return (mouseOverRowIndex, clickedRowIndex, clickedColumn);
        }
        private (int, float, int, ColumnDef?) DrawHeaders(
            Rect targetRect,
            Vector2 scrollPosition,
            float cellExtraWidth = 0f
        )
        {
            Widgets.DrawHighlight(targetRect);
            Widgets.DrawLineHorizontal(
                targetRect.x,
                targetRect.yMax,
                targetRect.width,
                StatsMainTabWindow.BorderLineColor
            );

            Widgets.BeginGroup(targetRect);

            var curX = -scrollPosition.x;
            var offset = curX;
            int indexStart = 0;
            int i = indexStart;
            ColumnDef? clickedColumn = null;

            for (; i < Columns.Count && curX < targetRect.width; i++)
            {
                var column = Columns[i];
                var columnWidth = Parent.ColumnMinWidths[column] + cellExtraWidth;

                if (curX + columnWidth <= 0f)
                {
                    curX += columnWidth;
                    offset = curX;
                    indexStart = i + 1;
                    continue;
                }

                var cellRect = new Rect(
                    curX,
                    0f,
                    columnWidth,
                    targetRect.height
                );

                if (DrawHeaderCell(cellRect, column))
                {
                    clickedColumn = column;
                }

                curX += columnWidth;
            }

            Widgets.EndGroup();

            return (indexStart, offset, i, clickedColumn);
        }
        private bool DrawHeaderCell(Rect targetRect, ColumnDef column)
        {
            var contentRect = targetRect.ContractedBy(CellPadding, 0);

            if (Parent.SortColumn == column)
            {
                Widgets.DrawBoxSolid(
                    Parent.SortDirection == SortDirection.Ascending
                        ? targetRect.BottomPartPixels(4f)
                        : targetRect.TopPartPixels(4f),
                    Color.yellow
                );
            }

            if (column.Icon != null)
            {
                var iconRect = column.CellTextAnchor switch
                {
                    TextAnchor.LowerRight => contentRect.RightPartPixels(RowHeight),
                    TextAnchor.LowerCenter => contentRect,
                    _ => contentRect.LeftPartPixels(RowHeight)
                };

                Widgets.DrawTextureFitted(iconRect, column.Icon, 1f);
            }
            else
            {
                using (new TextAnchorCtx(column.CellTextAnchor))
                {
                    Widgets.Label(contentRect, column.Label);
                }
            }

            TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));

            Widgets.DrawHighlightIfMouseover(targetRect);

            //LineVerticalWidget.Draw(
            //    targetRect.xMax,
            //    targetRect.y,
            //    targetRect.height,
            //    ColumnSeparatorLineColor
            //);

            return Widgets.ButtonInvisible(targetRect);
        }
        private (
            int? mouseOverRowIndex,
            int? clickedRowIndex
        ) DrawBody(
            Rect targetRect,
            Vector2 scrollPosition,
            float cellExtraWidth,
            int columnIndexStart,
            float offsetX,
            int columnIndexEnd
        )
        {
            Widgets.BeginGroup(targetRect);

            //float curSepX = -scrollPosition.x;
            //// Separators
            //for (int i = 0; i < Columns.Count - 1; i++)
            //{
            //    var column = Columns[i];
            //    var columnWidth = Parent.ColumnMinWidths[column] + cellExtraWidth;
            //    // Culling
            //    if (curSepX + columnWidth <= 0)
            //    {
            //        curSepX += columnWidth;
            //        continue;
            //    }
            //    else if (curSepX > targetRect.width)
            //    {
            //        break;
            //    }

            //    curSepX += columnWidth;

            //    LineVerticalWidget.Draw(
            //        curSepX,
            //        0f,
            //        targetRect.height,
            //        ColumnSeparatorLineColor
            //    );
            //}

            int debug_rowsDrawn = 0;
            int debug_columnsDrawn = 0;

            int? mouseOverRowIndex = null;
            int? clickedRowIndex = null;

            // Rows
            for (
                int rowIndex = (int)Math.Floor(scrollPosition.y / RowHeight);
                rowIndex < Math.Min(
                    (int)Math.Ceiling((scrollPosition.y + targetRect.height) / RowHeight),
                    Parent.Rows.Count
                );
                rowIndex++
            )
            {
                debug_columnsDrawn = 0;

                var row = Parent.Rows[rowIndex];
                var isEven = rowIndex % 2 == 0;
                var isMouseOver = Parent.MouseOverRowIndex == rowIndex;
                var rowRect = new Rect(
                    0f,
                    rowIndex * RowHeight - scrollPosition.y,
                    targetRect.width,
                    RowHeight
                );
                var curX = offsetX;

                if (isEven && !isMouseOver)
                {
                    Widgets.DrawLightHighlight(rowRect);
                }

                // Cells
                for (int i = columnIndexStart; i < columnIndexEnd; i++)
                {
                    var column = Columns[i];
                    var cellWidth = Parent.ColumnMinWidths[column] + cellExtraWidth;
                    var cellRect = new Rect(
                        curX,
                        rowRect.y,
                        cellWidth,
                        RowHeight
                    );

                    row.TryGetValue(column)?.Draw(
                        cellRect,
                        cellRect.ContractedBy(CellPadding, 0f),
                        column.CellTextAnchor
                    );

                    curX += cellRect.width;
                    debug_columnsDrawn++;
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

                debug_rowsDrawn++;
            }

            Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

            Widgets.EndGroup();

            return (mouseOverRowIndex, clickedRowIndex);
        }
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
