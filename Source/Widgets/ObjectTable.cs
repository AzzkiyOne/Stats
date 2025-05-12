using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class ObjectTable
{
    public abstract void Draw(Rect rect);
    public abstract void ResetFilters();
    public abstract TableFilterMode FilterMode { get; set; }
    public abstract void ToggleFilterMode();
    public abstract event Action<TableFilterMode> OnFilterModeChange;

    public enum TableFilterMode
    {
        AND = 0,
        OR = 1,
    }
}

internal sealed class ObjectTable<TObject> : ObjectTable
{
    private ColumnWorker<TObject> SortColumn;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly HashSet<FilterWidget<TObject>.AbsExpression> ActiveFilters;
    private readonly Table Table;
    private bool ShouldApplyFilters = false;
    private TableFilterMode _FilterMode;
    public override TableFilterMode FilterMode
    {
        get => _FilterMode;
        set
        {
            if (value == _FilterMode)
            {
                return;
            }

            _FilterMode = value;
            OnFilterModeChange?.Invoke(value);

            if (ActiveFilters.Count > 1)
            {
                ShouldApplyFilters = true;
            }
        }
    }
    public override event Action<TableFilterMode>? OnFilterModeChange;
    public ObjectTable(List<ColumnWorker<TObject>> columnWorkers, IEnumerable<TObject> records)
    {
        // We don't know what exactly tableDef.Worker.GetRecords() returns.
        // Could be a generator (and in some cases it is). So just in case
        // we "cache the returned collection".
        var recordsArr = records.ToArray();
        // Header rows and columns
        var columns = new List<Table.Column>(columnWorkers.Count);
        var headerRows = new List<TableRow>();
        var labelsRowCells = new List<Widget>(columnWorkers.Count);
        var filtersRowCells = new List<Widget>(columnWorkers.Count);
        ActiveFilters = new(columnWorkers.Count);
        SortColumn = columnWorkers.First();

        foreach (var columnWorker in columnWorkers)
        {
            var column = new Table.Column(
                columnWorker == SortColumn,
                (TextAnchor)columnWorker.CellStyle
            );
            columns.Add(column);

            labelsRowCells.Add(CreateHeaderCell(columnWorker, column));
            filtersRowCells.Add(CreateFilterCell(columnWorker, recordsArr, out var filter));

            filter.OnChange += HandleFilterChange;
        }

        var labelsRow = new TableRow(labelsRowCells, DrawHeaderRowBG);
        var filtersRow = new TableRow(filtersRowCells, DrawFiltersRowBG);

        headerRows.Add(labelsRow);
        headerRows.Add(filtersRow);

        // Body rows
        var bodyRows = new List<TableRow>(recordsArr.Length);

        foreach (var record in recordsArr)
        {
            var rowCells = new List<Widget>(columnWorkers.Count);

            for (int i = 0; i < columnWorkers.Count; i++)
            {
                var columnWorker = columnWorkers[i];
                var column = columns[i];

                rowCells.Add(CreateBodyCell(columnWorker, record));
            }

            var row = new TableRow<TObject>(rowCells, DrawBodyRowBG, record);

            bodyRows.Add(row);
        }

        // Finalize
        Table = new Table(columns, headerRows, bodyRows);

        SortRowsByColumn(SortColumn);
    }
    public override void Draw(Rect rect)
    {
        if (ShouldApplyFilters && Event.current.type == EventType.Layout)
        {
            ApplyFilters();
        }

        Table.Draw(rect);
    }
    private Widget CreateHeaderCell(ColumnWorker<TObject> columnWorker, Table.Column column)
    {
        void drawSortIndicator(Rect rect)
        {
            if (SortColumn == columnWorker && Event.current.type == EventType.Repaint)
            {
                if (SortDirection == SortDirectionAscending)
                {
                    rect.y = rect.yMax - 5f;
                    rect.height = 5f;
                }
                else
                {
                    rect.height = 5f;
                }

                Verse.Widgets.DrawBoxSolid(rect, SortIndicatorColor);
            }
        }
        void handleCellClick()
        {
            if (Event.current.control)
            {
                column.IsPinned = !column.IsPinned;
            }
            else
            {
                SortRowsByColumn(columnWorker);
            }
        }

        return columnWorker.ColumnDef.LabelFormat(columnWorker.ColumnDef, columnWorker.CellStyle)
            .PaddingAbs(cellPadHor, cellPadVer)
            .Background(drawSortIndicator)
            .ToButtonGhostly(
                handleCellClick,
                $"<i>{columnWorker.ColumnDef.LabelCap}</i>\n\n{columnWorker.ColumnDef.Description}"
            );
    }
    private static Widget CreateFilterCell(
        ColumnWorker<TObject> columnWorker,
        IEnumerable<TObject> records,
        out FilterWidget<TObject>.AbsExpression filter
    )
    {
        var filterWidget = columnWorker.GetFilterWidget(records);

        filter = filterWidget.Expression;
        return filterWidget;
    }
    private static Widget CreateBodyCell(ColumnWorker<TObject> columnWorker, TObject @object)
    {
        try
        {
            var cell = columnWorker.GetTableCellWidget(@object);

            if (cell == null)
            {
                return new EmptyWidget();
            }

            return cell.PaddingAbs(cellPadHor, cellPadVer);
        }
        catch (Exception e)
        {
            return new Label("!!!")
                .Color(Color.red.ToTransparent(0.5f))
                .TextAnchor(TextAnchor.LowerCenter)
                .PaddingAbs(cellPadHor, cellPadVer)
                .Tooltip(e.Message);
        }
    }
    private static void DrawHeaderRowBG(Rect rect, bool _, int __)
    {
        Verse.Widgets.DrawHighlight(rect);
    }
    private static void DrawFiltersRowBG(Rect rect, bool _, int __)
    {
    }
    private static void DrawBodyRowBG(Rect rect, bool isHovered, int index)
    {
        if (isHovered)
        {
            Verse.Widgets.DrawHighlight(rect);
        }
        else if (index % 2 == 0)
        {
            Verse.Widgets.DrawLightHighlight(rect);
        }
    }
    private void SortRowsByColumn(ColumnWorker<TObject> columnWorker)
    {
        if (SortColumn == columnWorker)
        {
            SortDirection *= -1;
        }
        else
        {
            SortColumn = columnWorker;
        }

        // TODO: Handle exception.
        Table.BodyRows.Sort((r1, r2) =>
            SortColumn.Compare(
                ((TableRow<TObject>)r1).Id,
                ((TableRow<TObject>)r2).Id
            ) * SortDirection
        );
    }
    private void HandleFilterChange(FilterWidget<TObject>.AbsExpression filter)
    {
        if (filter.IsEmpty)
        {
            ActiveFilters.Remove(filter);
        }
        else
        {
            ActiveFilters.Add(filter);
        }

        ShouldApplyFilters = true;
    }
    private void ApplyFilters()
    {
        // It is important not to skip pinned rows here so we don't have to
        // re-aplly filters when a row is unpinned.

        if (FilterMode == TableFilterMode.AND)
        {
            foreach (TableRow<TObject> row in Table.BodyRows)
            {
                row.IsHidden = ActiveFilters.Any(filter => filter.Eval(row.Id) == false);
            }
        }
        else
        {
            foreach (TableRow<TObject> row in Table.BodyRows)
            {
                row.IsHidden = !ActiveFilters.Any(filter => filter.Eval(row.Id));
            }
        }

        ShouldApplyFilters = false;
    }
    public override void ResetFilters()
    {
        if (ActiveFilters.Count == 0)
        {
            return;
        }

        foreach (var filter in ActiveFilters)
        {
            // If we handle OnChange event while iterating over active filters, the collection
            // will change because an incative filter will be removed on clear. This will cause an
            // exception.
            //
            // One solution here is to have separate OnClear event, that we just don't subscribe to.
            // The issue is, clear also causes change, so it would be confusing to have OnChange
            // event that is not emitted on clear.
            //
            // TODO: !!! This ofc is a hack, but i don't have a better solution currently.
            filter.OnChange -= HandleFilterChange;
            filter.Reset();
            filter.OnChange += HandleFilterChange;
        }

        ActiveFilters.Clear();

        foreach (TableRow<TObject> row in Table.BodyRows)
        {
            row.IsHidden = false;
        }
    }
    public override void ToggleFilterMode()
    {
        FilterMode = FilterMode switch
        {
            TableFilterMode.AND => TableFilterMode.OR,
            TableFilterMode.OR => TableFilterMode.AND,
            _ => throw new NotSupportedException("Unsupported table filtering mode."),
        };
    }
}
