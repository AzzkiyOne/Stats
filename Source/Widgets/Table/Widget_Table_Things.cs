using System.Collections.Generic;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table;

internal sealed class Widget_Table_Things
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private SortDirection SortDirection = SortDirection.Descending;
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    private readonly List<IThingMatcher> ThingMatchers = [];
    private readonly Widget_Table_Generic Table;
    private bool ShouldApplyFilters = false;
    public Widget_Table_Things(TableDef tableDef)
    {
        List<ColumnDef> columnDefs = [ColumnDefOf.Name, .. tableDef.columns];

        // Header rows and columns
        var columns = new List<Widget_Table_Generic.Column>();
        var headerRows = new List<Widget_TableRow>();
        var headerRow = new Widget_TableRow(DrawHeaderRowBG);
        var filtersRow = new Widget_TableRow(DrawFiltersRowBG);

        foreach (var columnDef in columnDefs)
        {
            var column = new Widget_Table_Generic.Column(columnDef == ColumnDefOf.Name);
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

        foreach (var rec in tableDef.Worker.GetRecords())
        {
            var row = new Widget_TableRow<ThingRec>(DrawBodyRowBG, rec);

            for (int i = 0; i < columnDefs.Count; i++)
            {
                var columnDef = columnDefs[i];
                var column = columns[i];

                row.Cells.Add(CreateBodyCell(columnDef, column, rec));
            }

            bodyRows.Add(row);
        }

        // Finalize
        Table = new Widget_Table_Generic(columns, headerRows, bodyRows);

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
        Widget_Table_Generic.Column column
    )
    {
        IWidget cell;

        if (columnDef.Icon != null)
        {
            cell = new Widget_Icon(columnDef.Icon);

            if (columnDef.Worker.CellStyle == ColumnCellStyle.Number)
            {
                new WidgetComp_Size_Inc_Rel(ref cell, 1f, 0f, 0f, 0f);
            }
            else if (columnDef.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                new WidgetComp_Size_Inc_Rel(ref cell, 0.5f, 0f);
            }

            cell = new Widget_Container_Single(cell);
        }
        else
        {
            cell = new Widget_Label(columnDef.labelShort);
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
                    SortDirection == SortDirection.Ascending
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
        new WidgetComp_Tooltip(ref cell, $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.description}");
        new WidgetComp_Bg_Tex_Hover(ref cell, TexUI.HighlightTex);
        new WidgetComp_OnClick(ref cell, handleCellClick);
        new WidgetComp_Generic(ref cell, drawSortIndicator);

        return new WidgetComp_TableCell_Normal(cell, column, columnDef.Worker.CellStyle);
    }
    private static WidgetComp_TableCell CreateFilterCell(
        ColumnDef columnDef,
        Widget_Table_Generic.Column column,
        out IThingMatcher thingMatcher
    )
    {
        IWidget_FilterInput filterWidget = columnDef.Worker.GetFilterWidget();
        var widget = (IWidget)filterWidget;
        new WidgetComp_Width_Rel(ref widget, 1f);

        thingMatcher = filterWidget.ThingMatcher;
        return new WidgetComp_TableCell_Normal(widget, column, columnDef.Worker.CellStyle);
    }
    private static WidgetComp_TableCell CreateBodyCell(
        ColumnDef columnDef,
        Widget_Table_Generic.Column column,
        ThingRec rec
    )
    {
        IWidget? cell = null;

        try
        {
            cell = columnDef.Worker.GetTableCellWidget(rec);
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
            SortDirection = (SortDirection)((int)SortDirection * -1);
        }
        else
        {
            SortColumn = columnDef;
        }

        Table.BodyRows.Sort((r1, r2) =>
            SortColumn.Worker.Compare(
                ((Widget_TableRow<ThingRec>)r1).Id,
                ((Widget_TableRow<ThingRec>)r2).Id
            ) * (int)SortDirection
        );
    }
    private void ScheduleFiltersApplication(IThingMatcher thingMatcher)
    {
        ShouldApplyFilters = true;
    }
    private void ApplyFilters()
    {
        // It is important not to skip selected rows here so we don't have to
        // re-aplly filters when a row is unselected.
        foreach (Widget_TableRow<ThingRec> row in Table.BodyRows)
        {
            row.IsHidden = !RowPassesFilters(row, ThingMatchers);
        }

        ShouldApplyFilters = false;
    }
    private static bool RowPassesFilters(
        Widget_TableRow<ThingRec> row,
        List<IThingMatcher> thingMatchers
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
