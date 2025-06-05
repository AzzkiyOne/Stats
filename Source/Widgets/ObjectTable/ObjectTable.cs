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

internal sealed partial class ObjectTable<TObject> : ObjectTable
{
    public ObjectTable(List<ColumnWorker<TObject>> columnWorkers, IEnumerable<TObject> objects)
    {
        // Columns
        var columnsCount = columnWorkers.Count;
        var columns = new Column[columnsCount];

        for (int i = 0; i < columnsCount; i++)
        {
            var columnWorker = columnWorkers[i];

            columns[i] = new Column(columnWorker, i == 0);
        }

        var sortColumn = columns[0];
        const int sorDirection = SortDirectionAscending;
        // TODO: Maybe the constructor should accept list from the beginning.
        var objectArr = objects.ToArray();
        // This is a bit of a hack. We rely on default sort direction being just what we need.
        Array.Sort(objectArr, sortColumn.Worker.Compare);

        // Headers
        var headerRows = new RowCollection<Row>(2);
        var columnLabelsRow = new ColumnLabelsRow(columns);
        var filtersRow = new Row(columns);

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            void drawSortIndicator(Rect rect)
            {
                if (SortColumn == column)
                {
                    if (SortDirection == SortDirectionAscending)
                    {
                        rect.y = rect.yMax - SortIndicatorHeight;
                        rect.height = SortIndicatorHeight;
                    }
                    else
                    {
                        rect.height = SortIndicatorHeight;
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
                    SortAllRowsByColumn(column);
                }
            }

            var columnDef = column.Worker.ColumnDef;

            // Label
            columnLabelsRow[i] = columnDef
                .LabelFormat(columnDef, column.Worker.CellStyle)
                .PaddingAbs(CellPadHor, CellPadVer)
                .Background(drawSortIndicator)
                .ToButtonGhostly(
                    handleCellClick,
                    $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.Description}"
                );

            // Filter
            var filterWidget = column.Worker.GetFilterWidget(objectArr);
            filterWidget.OnChange += filterWidget => HandleFilterChange(filterWidget, column);
            filtersRow[i] = filterWidget;
        }

        headerRows.Add(columnLabelsRow);
        headerRows.Add(filtersRow);

        // Body rows
        var bodyRows = new RowCollection<ObjectRow>(objectArr.Length);

        foreach (var @object in objectArr)
        {
            var row = new ObjectRow(columns, @object);

            for (int i = 0; i < columnsCount; i++)
            {
                var column = columns[i];

                try
                {
                    row[i] = column.Worker.GetTableCellWidget(@object)
                        ?.PaddingAbs(CellPadHor, CellPadVer);
                }
                catch (Exception e)
                {
                    row[i] = new Label("!!!")
                        .Color(Color.red.ToTransparent(0.5f))
                        .TextAnchor(TextAnchor.LowerCenter)
                        .PaddingAbs(CellPadHor, CellPadVer)
                        .Tooltip(e.Message);
                }
            }

            bodyRows.Add(row);
        }

        // Finalize
        Columns = columns;
        SortColumn = sortColumn;
        SortDirection = sorDirection;
        HeaderRows = headerRows;
        PinnedRows = new(10);
        UnfilteredBodyRows = bodyRows;
        FilteredBodyRows = new(bodyRows);
        ActiveFilters = new HashSet<FilterWidget<TObject>>(columnsCount);
        _FilterMode = TableFilterMode.AND;
        ObjectMatchesFilters = ObjectFilterMatchFuncAND;
        ShouldApplyFilters = false;
    }
}
