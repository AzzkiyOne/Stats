using System.Collections.Generic;
using Stats.Widgets.Containers;
using Stats.Widgets.Extensions;
using Stats.Widgets.Extensions.Size;
using Stats.Widgets.Extensions.Size.Constraints;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters;
using Stats.Widgets.Table.Filters.Widgets;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table;

internal sealed class ThingTable
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly List<IFilterExpression> ThingMatchers = [];
    private readonly GenericTable Table;
    private bool ShouldApplyFilters = false;
    public ThingTable(TableDef tableDef)
    {
        List<ColumnDef> columnDefs = [ColumnDefOf.Name, .. tableDef.columns];

        // Header rows and columns
        var columns = new List<GenericTable.Column>();
        var headerRows = new List<Widget_TableRow>();
        var headerRow = new Widget_TableRow(DrawHeaderRowBG);
        var filtersRow = new Widget_TableRow(DrawFiltersRowBG);

        foreach (var columnDef in columnDefs)
        {
            var column = new GenericTable.Column(columnDef == ColumnDefOf.Name);
            columns.Add(column);

            headerRow.Cells.Add(CreateHeaderCell(columnDef, column));
            filtersRow.Cells.Add(CreateFilterCell(columnDef, column, out var filterExpression));

            ThingMatchers.Add(filterExpression);
            filterExpression.OnChange += ScheduleFiltersApplication;
        }

        headerRows.Add(headerRow);
        headerRows.Add(filtersRow);

        // Body rows
        var bodyRows = new List<Widget_TableRow>();

        foreach (var thing in tableDef.Worker.GetRecords())
        {
            var row = new Widget_TableRow<ThingAlike>(DrawBodyRowBG, thing);

            for (int i = 0; i < columnDefs.Count; i++)
            {
                var columnDef = columnDefs[i];
                var column = columns[i];

                row.Cells.Add(CreateBodyCell(columnDef, column, thing));
            }

            bodyRows.Add(row);
        }

        // Finalize
        Table = new GenericTable(columns, headerRows, bodyRows);

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
    private WidgetComp_TableCell CreateHeaderCell(
        ColumnDef columnDef,
        GenericTable.Column column
    )
    {
        IWidget cell;

        if (columnDef.Icon != null)
        {
            cell = new Icon(columnDef.Icon);

            if (columnDef.Worker.CellStyle == ColumnCellStyle.Number)
            {
                cell = cell.PadRel(1f, 0f, 0f, 0f);
            }
            else if (columnDef.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                cell = cell.PadRel(0.5f, 0f);
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

        cell = cell
            .PadAbs(cellPadHor, cellPadVer)
            .WidthRel(1f)
            .Tooltip($"<i>{columnDef.LabelCap}</i>\n\n{columnDef.description}")
            .HoverBackground(TexUI.HighlightTex)
            .OnClick(handleCellClick)
            .Background(drawSortIndicator);

        return new WidgetComp_TableCell_Normal(
            cell,
            column,
            columnDef.Worker.CellStyle
        );
    }
    private static WidgetComp_TableCell CreateFilterCell(
        ColumnDef columnDef,
        GenericTable.Column column,
        out IFilterExpression filterExpression
    )
    {
        IFilterWidget filterWidget = columnDef.Worker.GetFilterWidget();

        filterExpression = filterWidget.FilterExpression;
        return new WidgetComp_TableCell_Normal(
            filterWidget.WidthRel(1f),
            column,
            columnDef.Worker.CellStyle
        );
    }
    private static WidgetComp_TableCell CreateBodyCell(
        ColumnDef columnDef,
        GenericTable.Column column,
        ThingAlike thing
    )
    {
        IWidget? cell = null;

        try
        {
            cell = columnDef.Worker.GetTableCellWidget(thing);
        }
        catch
        {
        }

        if (cell == null)
        {
            return new WidgetComp_TableCell_Empty(column);
        }
        else
        {
            return new WidgetComp_TableCell_Normal(
                cell.PadAbs(cellPadHor, cellPadVer).WidthRel(1f),
                column,
                columnDef.Worker.CellStyle);
        }
    }
    private static void DrawHeaderRowBG(Rect rect, bool _, int __)
    {
        Verse.Widgets.DrawHighlight(rect);
        //Verse.Widgets.DrawLineHorizontal(
        //    rect.x,
        //    rect.yMax - 1f,
        //    rect.width,
        //    StatsMainTabWindow.BorderLineColor
        //);
    }
    private static void DrawFiltersRowBG(Rect rect, bool _, int __)
    {
        //Verse.Widgets.DrawLineHorizontal(
        //    rect.x,
        //    rect.yMax - 1f,
        //    rect.width,
        //    StatsMainTabWindow.BorderLineColor
        //);
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
                ((Widget_TableRow<ThingAlike>)r1).Id,
                ((Widget_TableRow<ThingAlike>)r2).Id
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
        foreach (Widget_TableRow<ThingAlike> row in Table.BodyRows)
        {
            row.IsHidden = !RowPassesFilters(row, ThingMatchers);
        }

        ShouldApplyFilters = false;
    }
    private static bool RowPassesFilters(
        Widget_TableRow<ThingAlike> row,
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
