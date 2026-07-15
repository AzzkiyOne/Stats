using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Columns;
using Stats.Tables;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using Stats.Utils.Widgets;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Sound;
using static Stats.GUIStyles.Table;

namespace Stats;

public sealed partial class TableTab<TRecord>
{
    private void PinColumn(Column<TRecord> column)
    {
        int columnIndex = _columns.IndexOf(column);

        if (columnIndex != _leftColumnsCount)
        {
            int lastPinnedColumnIndex = _leftColumnsCount - 1;
            _columns.MoveAfterElemAt(columnIndex, lastPinnedColumnIndex);
        }

        _leftColumnsCount++;
    }

    private void UnpinColumn(Column<TRecord> column)
    {
        int columnIndex = _columns.IndexOf(column);
        int lastPinnedColumnIndex = _leftColumnsCount - 1;

        if (columnIndex != lastPinnedColumnIndex)
        {
            _columns.MoveAfterElemAt(columnIndex, lastPinnedColumnIndex);
        }

        _leftColumnsCount--;
    }

    private void HandleColumnDrag(Column<TRecord> draggedColumn, Column<TRecord> column, bool placeBefore)
    {
        int draggedColumnIndex = _columns.IndexOf(draggedColumn);
        int columnIndex = _columns.IndexOf(column);
        int leftColumnsCount = _leftColumnsCount;
        bool draggedColumnIsPinned = draggedColumnIndex < leftColumnsCount;
        bool columnIsPinned = columnIndex < leftColumnsCount;

        if (draggedColumnIsPinned && columnIsPinned == false)
        {
            _leftColumnsCount--;
        }
        else if (draggedColumnIsPinned == false && columnIsPinned)
        {
            _leftColumnsCount++;
        }

        if (placeBefore)
        {
            _columns.MoveBeforeElemAt(draggedColumnIndex, columnIndex);
        }
        else
        {
            _columns.MoveAfterElemAt(draggedColumnIndex, columnIndex);
        }
    }

    private Column<TRecord> AddColumn(ColumnDef columnDef, List<Column<TRecord>> columns, List<TRecord> records)
    {
        Column<TRecord> column = columnDef.MakeColumnInstance<TRecord>();
        columns.Add(column);
        records.ForEach(column.NotifyRecordAdded);

        column.OnPin += PinColumn;
        column.OnUnpin += UnpinColumn;
        column.OnRemove += RemoveColumn;

        return column;
    }

    private void AddColumn(ColumnDef columnDef)
    {
        Column<TRecord> column = AddColumn(columnDef, _columns, _records);
        _toolbar.NotifyColumnAdded(column);
    }

    private void RemoveColumn(int index)
    {
        if (index < _leftColumnsCount)
        {
            _leftColumnsCount--;
        }

        _toolbar.NotifyColumnRemoved(_columns[index]);
        _columns.RemoveAt(index);
    }

    private void RemoveColumn(Column<TRecord> column)
    {
        int index = _columns.IndexOf(column);
        RemoveColumn(index);
    }

    private void RemoveColumn(ColumnDef columnDef)
    {
        int index = _columns.FindIndex(column => column.Def == columnDef);
        RemoveColumn(index);
    }
}
