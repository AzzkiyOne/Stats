using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats;

internal abstract class ObjectTable
{
    internal abstract void Draw(Rect rect);

    internal abstract void NotifyParentWindowClosed();
}

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
internal sealed partial class ObjectTable<TRecord> : ObjectTable
{
    private static readonly TipSignal _manual =
        "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
        "  - You can pin multiple rows.\n" +
        "  - Pinned rows are unaffected by filters.\n" +
        "- Pull top part of the window to change height.\n" +
        "- Double click to reset window height.";
    // Filtering
    //public override TableFilterMode FilterMode
    //{
    //    get => field;
    //    set
    //    {
    //        if (value == field) return;

    //        field = value;
    //        MatchRowCells = value switch
    //        {
    //            TableFilterMode.AND => MatchRowCells_AND,
    //            TableFilterMode.OR => MatchRowCells_OR,
    //            _ => throw new NotSupportedException("Unsupported table filtering mode.")
    //        };

    //        OnFilterModeChange?.Invoke(value);
    //        DoFilter = true;
    //    }
    //} = TableFilterMode.AND;
    //public override event Action<TableFilterMode>? OnFilterModeChange;
    //private readonly List<Filter> Filters;
    //private readonly HashSet<Filter> ActiveFilters;
    //private RowCellsMatcher MatchRowCells = MatchRowCells_AND;
    //private static readonly RowCellsMatcher MatchRowCells_AND =
    //(cells, filters) =>
    //{
    //    return filters.All(filter => filter.Widget.Eval(cells[filter.Column]));
    //};
    //private static readonly RowCellsMatcher MatchRowCells_OR =
    //(cells, filters) =>
    //{
    //    return filters.Any(filter => filter.Widget.Eval(cells[filter.Column]));
    //};

    // Sorting
    private Column? _sortColumn;
    private int _sortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;

    // Filters tab

    // Rows
    private readonly List<TRecord> _objects;
    private readonly List<int> _rows;
    private int _topRowsCount;
    private int BottomRowsCount => _rows.Count - _topRowsCount;

    // Columns
    private readonly List<Column> _columns;
    private int _leftColumnsCount;
    private int RightColumnsCount => _columns.Count - _leftColumnsCount;
    private ReadOnlyListSegment<Column> LeftColumns => new(_columns, 0, _leftColumnsCount);
    private ReadOnlyListSegment<Column> RightColumns => new(_columns, _leftColumnsCount, RightColumnsCount);
    private Column? _reorderedColumn;

    // Layout
    private float _topRowsHeight;
    private float _bottomRowsHeight;
    private float _leftColumnsWidth;
    private Vector2 _contentSize;

    // Drawing
    private Vector2 _scrollPosition;
    // A way to defer any code that would otherwise modify
    // the collection that is currently being iterated over.
    // Primarily GUI event handlers.
    private Action? _beforeDraw;
    private bool _rightPartIsPanned;

    // Toolbar
    private readonly Toolbar _toolbar;

    // Misc
    private readonly TableWorker<TRecord> _tableWorker;

    public ObjectTable(TableWorker<TRecord> tableWorker)
    {
        //tableWorker.OnObjectAdded += AddObject;
        //tableWorker.OnObjectRemoved += RemoveObject;

        // Rows
        List<TRecord> objects = tableWorker.InitialRecords;
        int objectsCount = objects.Count;
        List<int> rows = new(objectsCount);
        for (int i = 0; i < objectsCount; i++)
        {
            rows.Add(i);
        }

        // Columns
        List<ColumnDef> columnDefs = tableWorker.Def.columns;
        int columnDefsCount = columnDefs.Count;
        List<Column> columns = new(columnDefsCount);
        for (int i = 0; i < columnDefsCount; i++)
        {
            ColumnDef columnDef = columnDefs[i];
            ColumnWorker<TRecord>? columnWorker = columnDef.TryMakeColumnWorkerInstance<TRecord>();

            if (columnWorker != null)
            {
                Column column = new(columnWorker, tableWorker, this);
                columns.Add(column);
                objects.ForEach(columnWorker.NotifyRecordAdded);
            }
            else
            {
                WarnIncompatibleColumn(columnDef.defName, tableWorker.Def.defName);
            }
        }

        // Finalize
        _tableWorker = tableWorker;
        _objects = objects;
        _rows = rows;
        _columns = columns;
        if (columns.Count > 0)
        {
            _leftColumnsCount = 1;
            _sortColumn = columns[0];
        }
        _toolbar = new Toolbar(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WarnIncompatibleColumn(string columnName, string tableName)
    {
        Log.Warning($"Column \"${columnName}\" is not compatible with table \"${tableName}\", because it does not implement \"${typeof(ColumnWorker<TRecord>).Name}\".");
    }

    internal override void NotifyParentWindowClosed()
    {
        _rightPartIsPanned = false;
        _reorderedColumn = null;
        for (int i = 0; i < _columns.Count; i++)
        {
            Column column = _columns[i];
            column.NotifyParentWindowClosed();
        }
    }
}
