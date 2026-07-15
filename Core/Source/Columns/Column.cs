using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Stats.Columns.Cells;
using Stats.Tables;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Stats.GUIStyles.Table;

namespace Stats.Columns;

public abstract class Column<TRecord>
{
    // TODO: Do we really need this to be public (or at all)?
    public ColumnDef Def { get; }
    internal float Width { get; private set; }

    internal event Action<Column<TRecord>>? OnRemove;
    internal event Action<Column<TRecord>>? OnPin;
    internal event Action<Column<TRecord>>? OnUnpin;

    private ColumnType _type { get; }
    private readonly Widget _labelWidget;
    private readonly TipSignal _tooltip;
    private readonly FloatMenu _menu;
    private bool _isResized;
    private bool _isManuallyResized;

    protected Column(ColumnDef def, ColumnType type)
    {
        Def = def;
        _type = type;
        _labelWidget = def.LabelWidget;
        _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.description}";
        _menu = new FloatMenu([
            //new FloatMenuOption("Sort Asc", () => {
            //    // TODO
            //}, TexButton.ReorderUp, Color.white),
            //new FloatMenuOption("Sort Desc", () => {
            //    // TODO
            //}, TexButton.ReorderDown, Color.white),
            new FloatMenuOption("Pin", () => OnPin?.Invoke(this)),
            new FloatMenuOption("Unpin", () => OnUnpin?.Invoke(this)),
            new FloatMenuOption("Remove", () => OnRemove?.Invoke(this), TexButton.Delete, Color.white)
        ]);
    }

    internal void Draw(Rect rect, Span<int> topRows, Span<int> bottomRows, float bottomRowsY, DragManager<Column<TRecord>> dragManager)
    {
        float topRowsHeight = topRows.Length * RowHeight;
        rect.CutTop(out Rect headerCellRect, HeadersRowHeight)
            .CutTop(out Rect topRowsRect, topRowsHeight)
            .TakeRest(out Rect bottomRowsRect);

        DrawHeaderCell(headerCellRect, dragManager);

        if (topRows.Length > 0)
        {
            using (new GUIClipScope(topRowsRect))
            {
                DrawCells(topRowsRect with { x = 0f, y = 0f }, topRows);
            }
        }

        if (bottomRows.Length > 0)
        {
            using (new GUIClipScope(bottomRowsRect, new Vector2(0f, bottomRowsY)))
            {
                DrawCells(bottomRowsRect with { x = 0f, y = 0f }, bottomRows);
            }
        }
    }

    protected abstract void DrawCell(Rect rect, int recordIndex);

    private void DrawCells(Rect rect, Span<int> rows)
    {
        ref Rect cellRect = ref rect;
        cellRect.height = RowHeight;
        int rowsCount = rows.Length;
        for (int i = 0; i < rowsCount; i++)
        {
            try
            {
                DrawCell(cellRect, rows[i]);
            }
            catch
            {
                // TODO:
                // - Add tooltip with exception's message.
                // - Make the whole thing into a separate non-inlineable method.
                cellRect.Fill(Color.red);
            }
            cellRect.y = cellRect.yMax;
        }
    }

    private void DrawHeaderCell(Rect rect, DragManager<Column<TRecord>> dragManager)
    {
        Event @event = Event.current;
        ColumnType columnType = _type;
        const float SideControlMargin = 1f;
        rect.CutLeft(out Rect sortControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
            .CutRight(out Rect resizeControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
            .TakeRest(out Rect labelControlRect);

        if (@event.type == EventType.Repaint)
        {
            Rect labelClipRect = labelControlRect.ContractedBy(SideControlMargin, GUIStyles.TableCell.PadVer);
            GUI.BeginClip(labelClipRect);

            float labelWidgetWidth = _labelWidget.Size.x;
            Rect labelRect = labelClipRect with { x = 0f, y = 0f };
            if (columnType == ColumnType.Number)
            {
                labelRect.CutRight(out labelRect, labelWidgetWidth);
            }
            else if (columnType == ColumnType.Boolean)
            {
                labelRect.CutMidX(out labelRect, labelWidgetWidth);
            }
            else
            {
                labelRect = labelRect with { width = labelWidgetWidth };
            }
            _labelWidget.Draw(labelRect);

            GUI.EndClip();

            if (dragManager.IsDragged(this))
            {
                rect.HighlightDragged();
            }
            else if (Mouse.IsOver(rect))
            {
                rect.HighlightLight();
            }

            rect.DrawBorderRight(ColumnSeparatorLineColor);
        }

        MouseoverSounds.DoRegion(rect);

        DoSortControl(sortControlRect);
        dragManager.OnGUI(labelControlRect, rect, this);
        DoLabelControl(labelControlRect);
        DoResizeControl(resizeControlRect);

        labelControlRect.Tip(_tooltip);
    }

    private void DoLabelControl(Rect rect)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseUp, button: 1, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            _menu.Open();
            GUIUtils.ReleaseMouseControl();
            //@event.Use();
        }

        rect.EmptyButton();
    }

    private void DoSortControl(Rect rect)
    {
        Event @event = Event.current;
        const float IconPadding = 3f;

        if (@event.type == EventType.Repaint)
        {
            // TODO
            //if (parent._sortColumn == this)
            //{
            //    if (parent._sortDirection == SortDirectionAscending)
            //    {
            //        rect.TopHalf()
            //            .ContractedBy(IconPadding)
            //            .DrawTextureFitted(TexButton.ReorderUp);
            //    }
            //    else
            //    {
            //        rect.BottomHalf()
            //            .ContractedBy(IconPadding)
            //            .DrawTextureFitted(TexButton.ReorderDown);
            //    }
            //}

            if (Mouse.IsOver(rect))
            {
                rect.Highlight();
            }
        }

        bool wasClicked = rect.EmptyButton();
        if (wasClicked && @event is { button: 0, modifiers: EventModifiers.None })
        {
            // TODO
            //if (parent._sortColumn != this)
            //{
            //    parent._sortColumn = this;
            //}
            //else
            //{
            //    parent._sortDirection *= -1;
            //}
        }
    }

    private void DoResizeControl(Rect rect)
    {
        Event @event = Event.current;
        bool mouseIsOverRect = Mouse.IsOver(rect);

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && mouseIsOverRect)
        {
            if (@event.clickCount > 1)
            {
                _isManuallyResized = false;
            }
            else
            {
                _isResized = true;
                _isManuallyResized = true;
            }
        }
        else if (_isResized)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                Width = Mathf.Clamp(Width + @event.delta.x, HeadersRowHeight, float.MaxValue);
                @event.Use();
            }
            // TODO: This will not work if the column will stop being rendered as the result of resizing.
            else if (@event.rawType == EventType.MouseUp)
            {
                _isResized = false;
                GUIUtils.ReleaseMouseControl();
                //@event.Use();
            }
        }

        if (@event.type == EventType.Repaint && (mouseIsOverRect || _isResized))
        {
            rect.HighlightDragged();
        }

        //GUI.SetNextControlName($"{Def.defName}_ColumnResizeControl");
        rect.EmptyButton();
    }

    protected abstract float GetMaxCellWidth(List<int> recordIndexes);

    internal void UpdateLayout(List<int> rows)
    {
        if (_isManuallyResized == false)
        {
            Width = Mathf.Max(_labelWidget.Size.x, GetMaxCellWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
        }
    }

    internal void Unfocus()
    {
        if (_isResized)
        {
            _isResized = false;
        }
    }

    public abstract void NotifyRecordAdded(TRecord record);

    public abstract void NotifyRecordRemoved(int recordIndex);

    public abstract void RefreshCells();

    public abstract ICollection<CellField> GetCellFields(Table tableWorker);
}

public abstract class Column<TRecord, TCell>(ColumnDef def, ColumnType type) :
    Column<TRecord>(def, type)
        where TCell :
            struct,
            ICell
{
    private readonly List<TCell> _cells = new(250);
    private int _refreshableCellsCount;

    protected TCell this[int index] => _cells[index];

    protected abstract TCell MakeCell(TRecord record);

    protected override void DrawCell(Rect rect, int recordIndex)
    {
        _cells[recordIndex].Draw(rect);
    }

    protected override float GetMaxCellWidth(List<int> recordIndexes)
    {
        float width = 0f;
        int recordsCount = recordIndexes.Count;
        for (int i = 0; i < recordsCount; i++)
        {
            int recordIndex = recordIndexes[i];
            float cellWidth = _cells[recordIndex].Width;
            if (width < cellWidth)
            {
                width = cellWidth;
            }
        }

        return width;
    }

    public override void NotifyRecordAdded(TRecord record)
    {
        TCell cell;
        try
        {
            cell = MakeCell(record);
        }
        catch
        {
            cell = default;
        }

        _cells.Add(cell);
        if (cell.IsRefreshable)
        {
            _refreshableCellsCount++;
        }
    }

    public override void NotifyRecordRemoved(int recordIndex)
    {
        if (_cells[recordIndex].IsRefreshable)
        {
            _refreshableCellsCount--;
        }
        _cells.ReplaceWithLast(recordIndex);
    }

    // TODO: Just let derived classes to directly write a cell if it was updated.
    protected virtual TCell RefreshCell(TCell cell, out bool wasStale)
    {
        wasStale = false;
        return cell;
    }

    public override void RefreshCells()
    {
        if (_refreshableCellsCount == 0)
        {
            return;
        }

        List<TCell> cells = _cells;
        int cellsCount = cells.Count;
        for (int i = 0; i < cellsCount; i++)
        {
            TCell originalCell = cells[i];
            if (originalCell.IsRefreshable)
            {
                TCell possiblyNewCell = RefreshCell(originalCell, out bool wasStale);
                if (wasStale)
                {
                    cells[i] = possiblyNewCell;
                }
            }
        }
    }
}
