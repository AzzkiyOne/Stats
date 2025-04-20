using System.Collections.Generic;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters;
using Stats.Widgets.Table.Filters.Widgets;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table;

internal sealed class ThingsTableWidget
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly List<IFilterExpression> ThingMatchers = [];
    private readonly GenericTableWidget Table;
    private bool ShouldApplyFilters = false;
    public ThingsTableWidget(TableDef tableDef)
    {
        List<ColumnDef> columnDefs = [ColumnDefOf.Name, .. tableDef.columns];

        // Header rows and columns
        var columns = new List<GenericTableWidget.Column>();
        var headerRows = new List<Widget_TableRow>();
        var headerRow = new Widget_TableRow(DrawHeaderRowBG);
        var filtersRow = new Widget_TableRow(DrawFiltersRowBG);

        foreach (var columnDef in columnDefs)
        {
            var column = new GenericTableWidget.Column(columnDef == ColumnDefOf.Name);
            columns.Add(column);

            headerRow.Cells.Add(CreateHeaderCell(columnDef, column));
            filtersRow.Cells.Add(CreateFilterCell(columnDef, column, out var tm));

            ThingMatchers.Add(tm);
            tm.OnChange += ScheduleFiltersApplication;
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
        Table = new GenericTableWidget(columns, headerRows, bodyRows);

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
        GenericTableWidget.Column column
    )
    {
        IWidget cell;

        if (columnDef.Icon != null)
        {
            cell = new IconWidget(columnDef.Icon);

            if (columnDef.Worker.CellStyle == ColumnCellStyle.Number)
            {
                new WidgetComp_Size_Inc_Rel(ref cell, 1f, 0f, 0f, 0f);
            }
            else if (columnDef.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                new WidgetComp_Size_Inc_Rel(ref cell, 0.5f, 0f);
            }

            cell = new SingleSlotContainerWidget(cell);
        }
        else
        {
            cell = new LabelWidget(columnDef.labelShort);
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

        new WidgetComp_Size_Inc_Abs(ref cell, cellPadHor, cellPadVer);
        new WidgetComp_Width_Rel(ref cell, 1f);
        new TooltipWidgetComp(ref cell, $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.description}");
        new TextureHoverWidgetComp(ref cell, TexUI.HighlightTex);
        new OnClickWidgetComp(ref cell, handleCellClick);
        new GenericWidgetComp(ref cell, drawSortIndicator);

        return new WidgetComp_TableCell_Normal(cell, column, columnDef.Worker.CellStyle);
    }
    private static WidgetComp_TableCell CreateFilterCell(
        ColumnDef columnDef,
        GenericTableWidget.Column column,
        out IFilterExpression thingMatcher
    )
    {
        IFilterWidget filterWidget = columnDef.Worker.GetFilterWidget();
        var widget = (IWidget)filterWidget;
        new WidgetComp_Width_Rel(ref widget, 1f);

        thingMatcher = filterWidget.FilterExpression;
        return new WidgetComp_TableCell_Normal(widget, column, columnDef.Worker.CellStyle);
    }
    private static WidgetComp_TableCell CreateBodyCell(
        ColumnDef columnDef,
        GenericTableWidget.Column column,
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
            new WidgetComp_Size_Inc_Abs(ref cell, cellPadHor, cellPadVer);
            new WidgetComp_Width_Rel(ref cell, 1f);

            return new WidgetComp_TableCell_Normal(cell, column, columnDef.Worker.CellStyle);
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
