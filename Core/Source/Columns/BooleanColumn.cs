using System.Collections.Generic;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.Tables;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class BooleanColumn<TRecord, TCell>(ColumnDef def) :
    Column<TRecord, TCell>(def, ColumnType.Boolean)
        where TCell :
            struct,
            IBooleanCell
{
    public override float GetWidth(List<int> rows)
    {
        return Verse.Text.LineHeight;
    }

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        Filter valueFieldFilter = new BooleanFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2]);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }
}

public abstract class BooleanColumn<TRecord>(ColumnDef def) :
    Column<TRecord>(def, ColumnType.Boolean)
{
    public override bool IsRefreshable => false;

    private readonly List<bool> _values = new(250);

    protected abstract bool GetValue(TRecord @object);

    public override void DrawCell(Rect rect, int row)
    {
        BooleanCell.Draw(rect, _values[row]);
    }

    public override float GetWidth(List<int> rows)
    {
        return Verse.Text.LineHeight;
    }

    public override void NotifyRecordAdded(TRecord row)
    {
        bool value;
        try
        {
            value = GetValue(row);
        }
        catch
        {
            value = default;
        }

        _values.Add(value);
    }

    public override void NotifyRecordRemoved(int row)
    {
        _values.ReplaceWithLast(row);
    }

    public override void RefreshCells() { }

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        Filter valueFieldFilter = new BooleanFilter((int row) => _values[row]);
        int Compare(int row1, int row2) => _values[row1].CompareTo(_values[row2]);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }
}
