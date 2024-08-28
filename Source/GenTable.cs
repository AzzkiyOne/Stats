﻿using System;
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
            middleColumnsWidth = 0;
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
                    middleColumnsWidth += column.MinWidth;
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
    private float middleColumnsWidth = 0f;
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
    private const float cellPadding = 5f;

    //private static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);

    public GenTable(List<ColumnType> columns, List<RowType> rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public void Draw(Rect targetRect)
    {
        using (new GameFontCtx(GameFont.Small))
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            var willHorScroll = totalRowsHeight + headersRowHeight > targetRect.height;
            var adjTargetRectWidth = willHorScroll
                ? targetRect.width - GenUI.ScrollBarWidth
                : targetRect.width;
            var contentRect = new Rect(
                0f,
                0f,
                Math.Max(minRowWidth, adjTargetRectWidth),
                totalRowsHeight + headersRowHeight
            );
            var extraCellWidth = CalcExtraMiddleCellsWidth(adjTargetRectWidth);

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
                adjTargetRectWidth,
                headersRowHeight
            );
            DrawHeaders(headersRect, extraCellWidth);

            var bodyRect = new Rect(
                scrollPosition.x,
                scrollPosition.y + headersRowHeight,
                adjTargetRectWidth,
                targetRect.height - headersRowHeight
            );
            DrawBody(bodyRect, extraCellWidth);

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
    private void DrawHeaders(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, pinnedLeftColumnsWidth),
            pinnedLeftColumns,
            Vector2.zero
        );
        // Draw middle headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition,
            extraCellWidth
        );
    }
    private void DrawHeaderColumns(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition.x;

        foreach (var column in columns)
        {
            var cellRect = new Rect(
                currX,
                0,
                column.MinWidth + extraCellWidth,
                targetRect.height
            );

            if (DrawHeaderCell(cellRect, column))
            {
                SortColumn = column;
            }

            currX += cellRect.width;
        }

        Widgets.EndGroup();
    }
    private bool DrawHeaderCell(Rect targetRect, ColumnType column)
    {
        //Widgets.DrawHighlight(targetRect);
        using (new TextAnchorCtx(
            column.Type == GenTable_ColumnType.Number
            ? TextAnchor.LowerRight
            : column.Type == GenTable_ColumnType.Boolean
            ? TextAnchor.LowerCenter
            : TextAnchor.LowerLeft
        ))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), column.Label);
        }

        if (SortColumn == column)
        {
            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(targetRect.height),
                TexButton.Reveal,
                (int)sortDirection * -90f
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
    private void DrawBody(Rect targetRect, float extraCellWidth)
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
            scrollPosition,
            extraCellWidth
        );
    }
    private void DrawRows(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
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

                var cellRect = new Rect(
                    currX,
                    currY,
                    column.MinWidth + extraCellWidth,
                    rowHeight
                );

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
                Widgets.Label(targetRect, cell.ValueStr);
                TooltipHandler.TipRegion(targetRect, new TipSignal(cell.Tip));
            }

            return;
        }

        if (cell.ValueStr == "")
        {
            return;
        }

        var label = Debug.InDebugMode ? cell.ValueNum.ToString() : cell.ValueStr;
        var tip = cell.Tip;

        if (
            // Not doing this check will (probably) impact performance.
            //column.Type == GenTable_ColumnType.Number
            //&& 
            SelectedRow is not null
            && row != SelectedRow
        )
        {
            cell.DiffCell = SelectedRow.GetCell(column);
            label = cell.ValueStrDiff;
            tip = cell.ValueStr;

            switch (cell.ValueNumDiff * column.DiffMult)
            {
                case < 0:
                    GUI.color = Color.red;
                    break;
                case > 0:
                    GUI.color = Color.green;
                    break;
                case 0:
                    GUI.color = Color.yellow;
                    break;
            }

            if (Debug.InDebugMode)
            {
                label = cell.ValueNumDiff.ToString();
            }
        }

        var contentRect = targetRect.ContractedBy(cellPadding, 0);
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

        using (new TextAnchorCtx(
            column.Type == GenTable_ColumnType.Number
            ? TextAnchor.LowerRight
            : column.Type == GenTable_ColumnType.Boolean
            ? TextAnchor.LowerCenter
            : TextAnchor.LowerLeft
        ))
        {
            Widgets.Label(contentRect.CutFromX(ref currX), label);
        }

        GUI.color = Color.white;

        if (
            //Event.current.control &&
            tip != ""
        )
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(tip));
        }
    }
    private float CalcExtraMiddleCellsWidth(float parentRectWidth)
    {
        parentRectWidth -= pinnedLeftColumnsWidth;

        if (middleColumnsWidth < parentRectWidth)
        {
            return (parentRectWidth - middleColumnsWidth) / middleColumns.Count;
        }

        return 0f;
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
    public int DiffMult { get; }
}

public abstract class GenTable_Column : IGenTable_Column
{
    private string _label = "";
    public string Label
    {
        get => _label;
        protected init
        {
            _label = value;
            MinWidth = Math.Max(Text.CalcSize(value).x + 15f, MinWidth);
        }
    }
    public string Description { get; protected init; } = "";
    private float _minWidth = 75f;
    public float MinWidth
    {
        get => _minWidth;
        protected init
        {
            _minWidth = Math.Max(Text.CalcSize(Label).x + 15f, value);
        }
    }
    public GenTable_ColumnType Type { get; protected init; } = GenTable_ColumnType.Number;
    public int DiffMult { get; protected init; } = 1;

    public GenTable_Column()
    {
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
    public IGenTable_Cell DiffCell { set; }
}

public abstract class GenTable_Cell : IGenTable_Cell
{
    public virtual float ValueNum { get; protected init; } = float.NaN;
    public string ValueStr { get; protected init; } = "";
    public virtual float ValueNumDiff { get; protected set; } = float.NaN;
    public string ValueStrDiff { get; protected set; } = "";
    private IGenTable_Cell? _diffCell = null;
    public virtual IGenTable_Cell DiffCell
    {
        set
        {
            if (value != _diffCell)
            {
                _diffCell = value;
                ValueNumDiff = ValueNum - value.ValueNum;
            }
        }
    }
    public string Tip { get; init; } = "";
    public Def? Def { get; init; } = null;
    public ThingDef? Stuff { get; init; } = null;

    public GenTable_Cell()
    {
    }

    public virtual int CompareTo(IGenTable_Cell other)
    {
        return ValueNum.CompareTo(other.ValueNum);
    }
}

public class GenTable_NumCell : GenTable_Cell
{
    private float _valueNum;
    public override float ValueNum
    {
        get => _valueNum;
        protected init
        {
            _valueNum = value;
            ValueStr = float.IsNaN(value) ? "" : value.ToString();
        }
    }
    private float _valueNumDiff;
    public override float ValueNumDiff
    {
        get => _valueNumDiff;
        protected set
        {
            _valueNumDiff = value;
            ValueStrDiff = float.IsNaN(value)
                ? ValueStr
                : value > 0
                    ? "+" + value.ToString()
                    : value.ToString();
        }
    }

    public GenTable_NumCell(float value = float.NaN)
    {
        ValueNum = value;
    }
}

public class GenTable_StrCell : GenTable_Cell
{
    public override IGenTable_Cell DiffCell { set { } }

    public GenTable_StrCell(string value = "")
    {
        ValueStr = value;
        // In "diff mode" the cell renderer uses ValueStrDiff to draw a label
        // regardless of whether a column's cells are diffable (because it doesn't know).
        // So this can be considered a crutch.
        ValueStrDiff = value;
    }

    public override int CompareTo(IGenTable_Cell other)
    {
        return ValueStr.CompareTo(other.ValueStr);
    }
}

public class GenTable_StatCell : GenTable_Cell
{
    private readonly StatDef stat;
    private StatRequest _req;
    private StatRequest Req
    {
        get => _req;
        init
        {
            _req = value;
            ValueNum = stat.Worker.GetValue(value);
        }
    }
    private float _valueNum;
    public override float ValueNum
    {
        get => _valueNum;
        protected init
        {
            _valueNum = value;
            ValueStr = float.IsNaN(value)
                ? ""
                : stat.Worker.GetStatDrawEntryLabel(
                    stat,
                    ValueNum,
                    ToStringNumberSense.Absolute,
                    Req
                );
        }
    }
    private float _valueNumDiff;
    public override float ValueNumDiff
    {
        get => _valueNumDiff;
        protected set
        {
            _valueNumDiff = value;

            if (float.IsNaN(value))
            {
                ValueStrDiff = ValueStr;
            }
            else
            {
                var strAbs = stat.Worker.GetStatDrawEntryLabel(
                stat,
                Math.Abs(value),
                ToStringNumberSense.Absolute,
                Req
            );

                // "Boolean" type cells are displayed incorrectly.
                // "No" (0) becomes "-Yes" (0 - 1 = -1).
                //
                // We have to keep in mind that string representation
                // of a stat's value wil not always be a formatted number.
                if (value > 0)
                {
                    strAbs = "+" + strAbs;
                }
                else if (value < 0)
                {
                    strAbs = "-" + strAbs;
                }

                ValueStrDiff = strAbs;
            }
        }
    }

    public GenTable_StatCell(
        StatDef stat,
        StatRequest req
    )
    {
        this.stat = stat;
        Req = req;
    }
}

public class GenTable_ExCell : GenTable_Cell
{
    public override IGenTable_Cell DiffCell { set { } }

    public GenTable_ExCell(Exception ex)
    {
        ValueStr = "!!!";
        Tip = ex.ToString();
    }
}

public class GenTable_BoolCell : GenTable_Cell
{
    public GenTable_BoolCell(float value)
    {
        ValueNum = value;

        if (value > 0)
        {
            ValueStr = ValueStrDiff = "Yes";
        }
        else if (value <= 0)
        {
            ValueStr = ValueStrDiff = "No";
        }
    }
}