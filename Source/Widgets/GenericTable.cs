using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class GenericTable<TObject> : ITableWidget
{
    private IColumnDef<TObject> SortColumn;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly HashSet<FilterWidget<TObject>.AbsExpression> ActiveFilters;
    private readonly Table Table;
    private bool ShouldApplyFilters = false;
    public GenericTable(ITableDef<TObject> tableDef)
    {
        // We don't know what exactly tableDef.Worker.GetRecords() returns.
        // Could be a generator (and in some cases it is). So just in case
        // we "cache the returned collection".
        var records = tableDef.Worker.GetRecords().ToArray();
        // Header rows and columns
        var columns = new List<Table.Column>(tableDef.Columns.Count);
        var headerRows = new List<TableRow>();
        var labelsRowCells = new List<Widget>(tableDef.Columns.Count);
        var filtersRowCells = new List<Widget>(tableDef.Columns.Count);
        ActiveFilters = new(tableDef.Columns.Count);
        SortColumn = tableDef.Columns.First();

        foreach (var columnDef in tableDef.Columns)
        {
            var column = new Table.Column(
                columnDef == SortColumn,
                (TextAnchor)columnDef.Worker.CellStyle
            );
            columns.Add(column);

            labelsRowCells.Add(CreateHeaderCell(columnDef, column));
            filtersRowCells.Add(CreateFilterCell(columnDef, records, out var filter));

            filter.OnChange += HandleFilterChange;
        }

        var labelsRow = new TableRow(labelsRowCells, DrawHeaderRowBG);
        var filtersRow = new TableRow(filtersRowCells, DrawFiltersRowBG);

        headerRows.Add(labelsRow);
        headerRows.Add(filtersRow);

        // Body rows
        var bodyRows = new List<TableRow>();

        foreach (var record in records)
        {
            var rowCells = new List<Widget>(tableDef.Columns.Count);

            for (int i = 0; i < tableDef.Columns.Count; i++)
            {
                var columnDef = tableDef.Columns[i];
                var column = columns[i];

                rowCells.Add(CreateBodyCell(columnDef, record));
            }

            var row = new TableRow<TObject>(rowCells, DrawBodyRowBG, record);

            bodyRows.Add(row);
        }

        // Finalize
        Table = new Table(columns, headerRows, bodyRows);

        SortRowsByColumn(SortColumn);
    }
    public void Draw(Rect rect)
    {
        if (ShouldApplyFilters && Event.current.type == EventType.Layout)
        {
            ApplyFilters();
        }

        Table.Draw(rect);
    }
    private Widget CreateHeaderCell(IColumnDef<TObject> columnDef, Table.Column column)
    {
        Widget cell;

        if (columnDef.Icon != null)
        {
            cell = new Icon(columnDef.Icon);

            if (columnDef.Worker.CellStyle == TableColumnCellStyle.Number)
            {
                cell = cell.PaddingRel(1f, 0f, 0f, 0f);
            }
            else if (columnDef.Worker.CellStyle == TableColumnCellStyle.Boolean)
            {
                cell = cell.PaddingRel(0.5f, 0f);
            }

            cell = new SingleElementContainer(cell);
        }
        else
        {
            cell = new Label(columnDef.LabelShort);
        }

        void drawSortIndicator(Rect rect)
        {
            if (SortColumn == columnDef && Event.current.type == EventType.Repaint)
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
                SortRowsByColumn(columnDef);
            }
        }

        return cell
            .PaddingAbs(cellPadHor, cellPadVer)
            .Background(drawSortIndicator)
            .ToButtonSubtle(
                handleCellClick,
                $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.Description}"
            );
    }
    private static Widget CreateFilterCell(
        IColumnDef<TObject> columnDef,
        IEnumerable<TObject> records,
        out FilterWidget<TObject>.AbsExpression filter
    )
    {
        var filterWidget = columnDef.Worker.GetFilterWidget(records);

        filter = filterWidget.Expression;
        return filterWidget;
    }
    private static Widget CreateBodyCell(IColumnDef<TObject> columnDef, TObject @object)
    {
        try
        {
            var cell = columnDef.Worker.GetTableCellWidget(@object);

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
    private void SortRowsByColumn(IColumnDef<TObject> columnDef)
    {
        if (SortColumn == columnDef)
        {
            SortDirection *= -1;
        }
        else
        {
            SortColumn = columnDef;
        }

        // TODO: Handle exception.
        Table.BodyRows.Sort((r1, r2) =>
            SortColumn.Worker.Compare(
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
        // It is important not to skip selected rows here so we don't have to
        // re-aplly filters when a row is unselected.
        foreach (TableRow<TObject> row in Table.BodyRows)
        {
            row.IsHidden = RowPassesFilters(row, ActiveFilters) == false;
        }

        ShouldApplyFilters = false;
    }
    private static bool RowPassesFilters(TableRow<TObject> row, HashSet<FilterWidget<TObject>.AbsExpression> filters)
    {
        foreach (var filter in filters)
        {
            if (filter.Eval(row.Id) == false)
            {
                return false;
            }
        }

        return true;
    }
    public void ClearFilters()
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
            filter.Clear();
            filter.OnChange += HandleFilterChange;
        }

        ActiveFilters.Clear();
        ShouldApplyFilters = true;
    }
}
