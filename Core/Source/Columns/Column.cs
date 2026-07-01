using System.Collections.Generic;
using Stats.Columns.Cells;
using Stats.Tables;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class Column<TRecord>
{
    public ColumnDef Def { get; }
    public ColumnType Type { get; }
    public abstract bool IsRefreshable { get; }
    public virtual bool ShouldDrawCellsNow => Event.current.type == EventType.Repaint;

    protected Column(ColumnDef def, ColumnType type)
    {
        Def = def;
        Type = type;
    }

    public abstract void DrawCell(Rect rect, int recordIndex);

    public abstract float GetWidth(List<int> recordIndexes);

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
    public override bool IsRefreshable => _refreshableCellsCount > 0;

    private readonly List<TCell> _cells = new(250);
    private int _refreshableCellsCount;

    protected TCell this[int index] => _cells[index];

    protected abstract TCell MakeCell(TRecord record);

    public override void DrawCell(Rect rect, int recordIndex)
    {
        _cells[recordIndex].Draw(rect);
    }

    public override float GetWidth(List<int> recordIndexes)
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
