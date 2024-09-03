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
    where ColumnType : ColumnDef
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

            foreach (var column in value)
            {
                if (column.isPinned)
                {
                    pinnedLeftColumns.Add(column);
                    pinnedLeftColumnsWidth += column.minWidth;
                }
                else
                {
                    middleColumns.Add(column);
                    middleColumnsWidth += column.minWidth;
                }

                minRowWidth += column.minWidth;
            }

            if (SortColumn is null && pinnedLeftColumns.First() != null)
            {
                SortColumn = pinnedLeftColumns.First();
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
                column.minWidth + extraCellWidth,
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
        using (new TextAnchorCtx(column.textAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), column.LabelCap);
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

        TooltipHandler.TipRegion(targetRect, new TipSignal(column.description));

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
                if (currX + column.minWidth <= 0)
                {
                    currX += column.minWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = new Rect(
                    currX,
                    currY,
                    column.minWidth + extraCellWidth,
                    rowHeight
                );
                var cell = row.GetCell(column);
                var diffCell = SelectedRow?.GetCell(column);

                if (cell is not null)
                {
                    DrawRowCell(cellRect, cell, diffCell);
                }

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
    private void DrawRowCell(Rect targetRect, Cell cell, Cell? diffCell)
    {
        if (cell.BGColor is Color bgColor)
        {
            using (new ColorCtx(bgColor))
            {
                Widgets.DrawHighlight(targetRect);
            }
        }

        if (Event.current.type == EventType.Repaint)
        {
            cell.SwitchToDiffState(diffCell);
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

        using (new ColorCtx(cell.Color))
        using (new TextAnchorCtx(cell.TextAnchor))
        {
            var label = Debug.InDebugMode ? cell.Value.ToString() : cell.Label;
            Widgets.Label(contentRect.CutFromX(ref currX), label);
        }

        if (cell.Tip != "")
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(cell.Tip));
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
        {
            var r1c = r1.GetCell(SortColumn);
            var r2c = r2.GetCell(SortColumn);
            int result = 0;

            if (r1c is not null && r2c is not null)
            {
                result = r1c.CompareTo(r2c);
            }
            else if (r1c is null && r2c is not null)
            {
                result = -1;
            }
            else if (r1c is not null && r2c is null)
            {
                result = 1;
            }

            return result * (int)sortDirection;
        });
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
    //public string Label { get; }
    public string Description { get; }
    //public float MinWidth { get; }
    //public GenTable_ColumnType Type { get; }
    //public int DiffMult { get; }
    public ColumnDef Def { get; }
}

public abstract class GenTable_Column : IGenTable_Column
{
    //private string _label = "";
    //public string Label
    //{
    //    get => _label;
    //    protected init
    //    {
    //        _label = value;
    //        MinWidth = Math.Max(Text.CalcSize(value).x + 15f, MinWidth);
    //    }
    //}
    public string Description { get; protected init; } = "";
    //private float _minWidth = 75f;
    //public float MinWidth
    //{
    //    get => _minWidth;
    //    protected init
    //    {
    //        _minWidth = Math.Max(Text.CalcSize(Label).x + 15f, value);
    //    }
    //}
    //public GenTable_ColumnType Type { get; protected init; } = GenTable_ColumnType.Number;
    //public int DiffMult { get; protected init; } = 1;
    public ColumnDef Def { get; }

    public GenTable_Column(ColumnDef def)
    {
        Def = def;
    }
}

public interface IGenTable_Row<ColumnType> where ColumnType : ColumnDef
{
    public Cell? GetCell(ColumnType column);
}

public abstract class Cell : IComparable<Cell>
{
    public StrOrSingle Value { get; set; } = float.NaN;
    public string Label { get; set; } = "";
    public string Tip { get; set; } = "";
    public Def? Def { get; set; }
    public ThingDef? Stuff { get; set; }
    public Color Color { get; set; } = Color.white;
    public Color? BGColor { get; set; }
    public TextAnchor TextAnchor { get; init; }
    private int DiffMult { get; init; }
    private Cell? curDiffCell;

    public Cell(ColumnDef columnDef)
    {
        TextAnchor = columnDef.textAnchor;
        DiffMult = columnDef.reverseDiffModeColors ? -1 : 1;
    }

    public void SwitchToDiffState(Cell? cell)
    {
        if (curDiffCell == cell)
        {
            return;
        }

        curDiffCell = cell;

        if (cell is null || cell == this)
        {
            SwitchToNormalState();
        }
        else
        {
            SwitchToDiffState(Value.Single - cell.Value.Single);
        }
    }
    public int CompareTo(Cell other)
    {
        return Value.CompareTo(other.Value);
    }

    protected virtual void SwitchToDiffState(float value)
    {
        switch (value * DiffMult)
        {
            case < 0:
                Color = Color.red;
                break;
            case > 0:
                Color = Color.green;
                break;
            case 0:
                Color = Color.yellow;
                break;
        }
    }
    protected virtual void SwitchToNormalState()
    {
        Color = Color.white;
    }
}

public class NumCell : Cell
{
    public NumCell(ColumnDef columnDef, float value = float.NaN) : base(columnDef)
    {
        Value = value;
        Label = Value.ToString();
    }

    protected override void SwitchToDiffState(float value)
    {
        base.SwitchToDiffState(value);

        Label = value > 0 ? "+" + value : value.ToString();
        Tip = Value.ToString();
    }
    protected override void SwitchToNormalState()
    {
        base.SwitchToNormalState();

        Label = Value.ToString();
        Tip = "";
    }
}

public class StrCell : Cell
{
    public StrCell(ColumnDef columnDef, string value = "") : base(columnDef)
    {
        Value = value;
        Label = value;
    }

    protected override void SwitchToDiffState(float value)
    {
    }
    protected override void SwitchToNormalState()
    {
    }
}

public class BoolCell : Cell
{
    public BoolCell(ColumnDef columnDef, float value) : base(columnDef)
    {
        Value = value;
        TextAnchor = TextAnchor.MiddleCenter;

        if (value > 0)
        {
            Label = "Yes";
        }
        else if (value <= 0)
        {
            Label = "No";
        }
    }
}

public class ExCell : Cell
{
    public ExCell(ColumnDef columnDef, Exception ex) : base(columnDef)
    {
        // This cell may appear in a column of any type.
        // This can cause exception in StrOrSingle.CompareTo
        Value = float.NaN;
        Label = "!!!";
        TextAnchor = TextAnchor.MiddleCenter;
        Tip = ex.ToString();
        BGColor = Color.red;
    }

    protected override void SwitchToDiffState(float value)
    {
    }
    protected override void SwitchToNormalState()
    {
    }
}

public class StatCell : Cell
{
    private readonly StatDef stat;
    private readonly StatRequest req;

    public StatCell(
        ColumnDef columnDef,
        StatDef stat,
        StatRequest req
    ) : base(columnDef)
    {
        this.stat = stat;
        this.req = req;
        Value = stat.Worker.GetValue(req);
        Label = float.IsNaN(Value.Single)
            ? ""
            : stat.Worker.GetStatDrawEntryLabel(
                stat,
                Value.Single,
                ToStringNumberSense.Absolute,
                req
            );
    }

    protected override void SwitchToDiffState(float value)
    {
        base.SwitchToDiffState(value);

        Tip = float.IsNaN(Value.Single)
            ? ""
            : stat.Worker.GetStatDrawEntryLabel(
                stat,
                Value.Single,
                ToStringNumberSense.Absolute,
                req
            );

        if (float.IsNaN(value))
        {
            Label = float.IsNaN(Value.Single)
                ? ""
                : stat.Worker.GetStatDrawEntryLabel(
                    stat,
                    Value.Single,
                    ToStringNumberSense.Absolute,
                    req
                );
        }
        else
        {
            var strAbs = stat.Worker.GetStatDrawEntryLabel(
                stat,
                Math.Abs(value),
                ToStringNumberSense.Absolute,
                req
            );

            // "Boolean" type cells are displayed incorrectly.
            // "No" (0) becomes "-Yes" (0 - 1 = -1).
            //
            // We have to keep in mind that string representation
            // of a stat's value will not always be a formatted number.
            if (value > 0)
            {
                strAbs = "+" + strAbs;
            }
            else if (value < 0)
            {
                strAbs = "-" + strAbs;
            }

            Label = strAbs;
        }
    }
    protected override void SwitchToNormalState()
    {
        base.SwitchToNormalState();

        Label = float.IsNaN(Value.Single)
            ? ""
            : stat.Worker.GetStatDrawEntryLabel(
                stat,
                Value.Single,
                ToStringNumberSense.Absolute,
                req
            );
        Tip = "";
    }
}
