using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class ThingTable
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly List<IFilterExpression> ThingMatchers = [];
    private readonly Table Table;
    private bool ShouldApplyFilters = false;
    public ThingTable(TableDef tableDef)
    {
        List<ColumnDef> columnDefs = [ColumnDefOf.Name, .. tableDef.columns];

        // Header rows and columns
        var columns = new List<Table.Column>();
        var headerRows = new List<TableRow>();
        var headerRow = new TableRow(DrawHeaderRowBG);
        var filtersRow = new TableRow(DrawFiltersRowBG);

        foreach (var columnDef in columnDefs)
        {
            var column = new Table.Column(columnDef == ColumnDefOf.Name);
            columns.Add(column);

            headerRow.Cells.Add(CreateHeaderCell(columnDef, column));
            filtersRow.Cells.Add(CreateFilterCell(columnDef, out var filterExpression));

            ThingMatchers.Add(filterExpression);
            filterExpression.OnChange += ScheduleFiltersApplication;
        }

        headerRows.Add(headerRow);
        headerRows.Add(filtersRow);

        // Body rows
        var bodyRows = new List<TableRow>();

        foreach (var thing in tableDef.Worker.GetRecords())
        {
            var row = new TableRow<ThingAlike>(DrawBodyRowBG, thing);

            for (int i = 0; i < columnDefs.Count; i++)
            {
                var columnDef = columnDefs[i];
                var column = columns[i];

                row.Cells.Add(CreateBodyCell(columnDef, thing));
            }

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
    private IWidget CreateHeaderCell(
        ColumnDef columnDef,
        Table.Column column
    )
    {
        IWidget cell;

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
            cell = new Label(columnDef.labelShort);
        }

        void drawSortIndicator(Rect rect)
        {
            if
            (
                Event.current.type == EventType.Repaint
                &&
                SortColumn == columnDef
            )
            {
                Verse.Widgets.DrawBoxSolid(
                    SortDirection == SortDirectionAscending
                        ? rect.BottomPartPixels(5f)
                        : rect.TopPartPixels(5f),
                    Color.yellow.ToTransparent(0.3f)
                );
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
            .TextAnchor((TextAnchor)columnDef.Worker.CellStyle)
            .PaddingAbs(cellPadHor, cellPadVer)
            .WidthRel(1f)
            .Background(drawSortIndicator)
            .ToButtonSubtle(
                handleCellClick,
                $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.description}"
            );
    }
    private static IWidget CreateFilterCell(
        ColumnDef columnDef,
        out IFilterExpression filterExpression
    )
    {
        var filterWidget = columnDef.Worker.GetFilterWidget();

        filterExpression = filterWidget.FilterExpression;
        return filterWidget
            .TextAnchor((TextAnchor)columnDef.Worker.CellStyle)
            .WidthRel(1f);
    }
    private static IWidget CreateBodyCell(
        ColumnDef columnDef,
        ThingAlike thing
    )
    {
        try
        {
            var cell = columnDef.Worker.GetTableCellWidget(thing);

            if (cell != null)
            {
                return cell
                    .TextAnchor((TextAnchor)columnDef.Worker.CellStyle)
                    .PaddingAbs(cellPadHor, cellPadVer)
                    .WidthRel(1f);
            }
        }
        catch
        {
        }

        return new EmptyWidget();
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
    private void SortRowsByColumn(ColumnDef columnDef)
    {
        if (SortColumn == columnDef)
        {
            SortDirection *= -1;
        }
        else
        {
            SortColumn = columnDef;
        }

        Table.BodyRows.Sort((r1, r2) =>
            SortColumn.Worker.Compare(
                ((TableRow<ThingAlike>)r1).Id,
                ((TableRow<ThingAlike>)r2).Id
            ) * SortDirection
        );
    }
    private void ScheduleFiltersApplication(IFilterExpression thingMatcher)
    {
        ShouldApplyFilters = true;
    }
    private void ApplyFilters()
    {
        // It is important not to skip selected rows here so we don't have to
        // re-aplly filters when a row is unselected.
        foreach (TableRow<ThingAlike> row in Table.BodyRows)
        {
            row.IsHidden = !RowPassesFilters(row, ThingMatchers);
        }

        ShouldApplyFilters = false;
    }
    private static bool RowPassesFilters(
        TableRow<ThingAlike> row,
        List<IFilterExpression> thingMatchers
    )
    {
        // Why to evaluate all thing matchers? We can make a list of "active"
        // thing matchers and only evaluate those.
        foreach (var thingMatcher in thingMatchers)
        {
            if (thingMatcher.Match(row.Id) == false)
            {
                return false;
            }
        }

        return true;
    }
    public void ResetFilters()
    {
        foreach (var thingMatcher in ThingMatchers)
        {
            thingMatcher.Reset();
        }
    }
}
