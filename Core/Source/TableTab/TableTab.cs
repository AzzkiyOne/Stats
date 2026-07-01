using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Columns;
using Stats.Tables;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats;

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
public sealed partial class TableTab<TRecord> :
    MainTabWindowTab
{
    public event Action? OnDispose;

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
    private ColumnWidget? _sortColumn;
    private int _sortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;

    // Filters tab

    // Rows
    private readonly List<TRecord> _records;
    private readonly List<int> _rows;
    private int _topRowsCount;
    private int BottomRowsCount => _rows.Count - _topRowsCount;

    // Columns
    private readonly List<ColumnDef> _columnDefs;
    private readonly List<ColumnWidget> _columns;
    private int _leftColumnsCount;
    private int RightColumnsCount => _columns.Count - _leftColumnsCount;
    private ReadOnlyListSegment<ColumnWidget> LeftColumns => new(_columns, 0, _leftColumnsCount);
    private ReadOnlyListSegment<ColumnWidget> RightColumns => new(_columns, _leftColumnsCount, RightColumnsCount);
    private ColumnWidget? _reorderedColumn;

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

    public TableTab(TableDef def, List<TRecord> records) : base(def)
    {
        // Rows
        int recordsCount = records.Count;
        List<int> rows = new(recordsCount);
        for (int i = 0; i < recordsCount; i++)
        {
            rows.Add(i);
        }

        // Columns
        List<ColumnDef> initialColumnDefs = def.columns;
        int initialColumnDefsCount = initialColumnDefs.Count;
        List<ColumnWidget> columns = new(initialColumnDefsCount);
        for (int i = 0; i < initialColumnDefsCount; i++)
        {
            ColumnDef columnDef = initialColumnDefs[i];
            try
            {
                Column<TRecord> column = columnDef.MakeColumnInstance<TRecord>();
                ColumnWidget columnWrapper = new(column, this);
                columns.Add(columnWrapper);
                records.ForEach(column.NotifyRecordAdded);
            }
            catch (Exception error)
            {
                Log.Error(error.Message);
            }
        }

        // Finalize
        _records = records;
        _rows = rows;
        _columnDefs = def.CompatibleColumns;
        _columns = columns;
        if (columns.Count > 0)
        {
            _leftColumnsCount = 1;
            _sortColumn = columns[0];
        }
        _toolbar = new Toolbar(this);
    }

    public override void Focus()
    {
        // TODO?
    }

    public override void Unfocus()
    {
        _rightPartIsPanned = false;
        _reorderedColumn = null;
        for (int i = 0; i < _columns.Count; i++)
        {
            ColumnWidget column = _columns[i];
            column.Unfocus();
        }
    }

    public void AddRecord(TRecord record)
    {
        // TODO
    }

    public void RemoveRecord(TRecord record)
    {
        // TODO
    }

    public override void Dispose()
    {
        OnDispose?.Invoke();
    }
}
