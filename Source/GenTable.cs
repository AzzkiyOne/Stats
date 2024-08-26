using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

internal class GenTable<ColumnType, RowType>
    where ColumnType : class, IGenTable_Column
    where RowType : class, IGenTable_Row<ColumnType>
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
        var cell = row.GetCell(column);

        if (cell is GenTable_ExCell)
        {
            GUI.color = Color.red;
            Widgets.DrawHighlight(targetRect);
            GUI.color = Color.white;

            using (new TextAnchorCtx(TextAnchor.MiddleCenter))
            {
                Widgets.Label(targetRect, "!!!");
            }

            return;
        }

        if (cell.ValueStr == "")
        {
            return;
        }

        var label = cell.ValueStr;
        var tip = cell.Tip;

        if (
            column.Type == GenTable_ColumnType.Number
            && SelectedRow is not null
            && row != SelectedRow
        )
        {
            var selectedRowCell = SelectedRow.GetCell(column);
            var thisCell = row.GetCell(column);

            thisCell.Diff(selectedRowCell);
            label = thisCell.ValueStrDiff;
            tip = thisCell.ValueStr;

            switch (thisCell.ValueNumDiff)
            {
                case < 0:
                    GUI.color = Color.red;
                    break;
                case > 0:
                    GUI.color = Color.green;
                    break;
                default:
                    GUI.color = Color.yellow;
                    break;
            }
        }

        var contentRect = targetRect.ContractedBy(GenUI.Pad, 0);
        var currX = contentRect.x;

        if (cell.Def is not null)
        {
            // This is very expensive.
            Widgets.DefIcon(
                contentRect.CutFromX(ref currX, contentRect.height),
                cell.Def,
                cell.Stuff
            );

            currX += GenUI.Pad;

            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                GUIWidgets.DefInfoDialog(cell.Def, cell.Stuff);
            }
        }

        Widgets.Label(contentRect.CutFromX(ref currX), label);

        GUI.color = Color.white;

        if (
            //Event.current.control &&
            tip != ""
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

        Rows.Sort((r1, r2) =>
            r1.GetCell(SortColumn).CompareTo(r2.GetCell(SortColumn)) * (int)sortDirection
        );
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}

public enum GenTable_ColumnType
{
    Number,
    String,
    Boolean,
}

public interface IGenTable_Column
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public GenTable_ColumnType Type { get; }
}

public abstract class GenTable_Column : IGenTable_Column
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public GenTable_ColumnType Type { get; }

    public GenTable_Column(
        string? label = null,
        string? description = null,
        float? minWidth = null,
        GenTable_ColumnType type = GenTable_ColumnType.Number
    )
    {
        Label = label ?? "";
        Description = description ?? "";
        MinWidth = minWidth ?? 100f;
        Type = type;
    }
}

public interface IGenTable_Row<ColumnType> where ColumnType : IGenTable_Column
{
    public IGenTable_Cell GetCell(ColumnType column);
}

public interface IGenTable_Cell : IComparable<IGenTable_Cell>
{
    public float ValueNum { get; }
    public string ValueStr { get; }
    public float ValueNumDiff { get; }
    public string ValueStrDiff { get; }
    public string Tip { get; }
    public Def? Def { get; }
    public ThingDef? Stuff { get; }
    public void Diff(IGenTable_Cell other);
}

public abstract class GenTable_Cell : IGenTable_Cell
{
    public float ValueNum { get; init; } = float.NaN;
    public string ValueStr { get; init; } = "";
    public float ValueNumDiff { get; set; } = float.NaN;
    public string ValueStrDiff { get; set; } = "";
    protected IGenTable_Cell? DiffCell { get; set; } = null;
    public string Tip { get; init; } = "";
    public Def? Def { get; init; }
    public ThingDef? Stuff { get; init; }

    public GenTable_Cell()
    {
    }

    public abstract void Diff(IGenTable_Cell other);

    public abstract int CompareTo(IGenTable_Cell other);
}

public class GenTable_NumCell : GenTable_Cell
{
    public GenTable_NumCell()
    {
    }

    public override int CompareTo(IGenTable_Cell other)
    {
        return ValueNum.CompareTo(other.ValueNum);
    }
    public override void Diff(IGenTable_Cell other)
    {
        if (DiffCell != other)
        {
            DiffCell = other;
            ValueNumDiff = ValueNum - other.ValueNum;
            ValueStrDiff = ValueNumDiff > 0
                ? "+" + ValueNumDiff.ToString()
                : ValueNumDiff.ToString();
        }
    }
}

public class GenTable_StrCell : GenTable_Cell
{
    public GenTable_StrCell()
    {
    }

    public override int CompareTo(IGenTable_Cell other)
    {
        return ValueStr.CompareTo(other.ValueStr);
    }
    public override void Diff(IGenTable_Cell other)
    {
    }
}

public class GenTable_StatCell : GenTable_Cell
{
    private readonly StatDef stat;
    private readonly StatRequest req;

    public GenTable_StatCell(
        StatDef stat,
        StatRequest req
    )
    {
        this.stat = stat;
        this.req = req;
        ValueNum = stat.Worker.GetValue(req);
        ValueStr = stat.Worker.GetStatDrawEntryLabel(
            stat,
            ValueNum,
            ToStringNumberSense.Absolute,
            req
        );
    }

    public override int CompareTo(IGenTable_Cell other)
    {
        return ValueNum.CompareTo(other.ValueNum);
    }
    public override void Diff(IGenTable_Cell other)
    {
        if (DiffCell != other)
        {
            DiffCell = other;
            ValueNumDiff = ValueNum - other.ValueNum;

            var strAbs = stat.Worker.GetStatDrawEntryLabel(
                stat,
                Math.Abs(ValueNumDiff),
                ToStringNumberSense.Absolute,
                req
            );

            // "Boolean" type cells are displayed incorrectly.
            // "No" (0) becomes "-Yes" (0 - 1 = -1).
            //
            // We have to keep in mind that string representation
            // of a stat's value wil not always be a formatted number.
            if (ValueNumDiff > 0)
            {
                strAbs = "+" + strAbs;
            }
            else if (ValueNumDiff < 0)
            {
                strAbs = "-" + strAbs;
            }

            ValueStrDiff = strAbs;
        }
    }
}

public class GenTable_ExCell : GenTable_Cell
{
    public GenTable_ExCell()
    {
    }

    public override int CompareTo(IGenTable_Cell other)
    {
        return ValueNum.CompareTo(other.ValueNum);
    }
    public override void Diff(IGenTable_Cell other)
    {
    }
}