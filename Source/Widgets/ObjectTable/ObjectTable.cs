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
    private readonly Column[] Columns;
    private readonly List<Row> HeaderRows = [];
    private readonly List<BodyRow> BodyRows = [];
    public ObjectTable(List<ColumnWorker<TObject>> columnWorkers, IEnumerable<TObject> records)
    {
        // Columns
        var columnsCount = columnWorkers.Count;
        SortColumn = columnWorkers.First();
        Columns = new Column[columnsCount];

        for (int i = 0; i < columnsCount; i++)
        {
            var columnWorker = columnWorkers[i];
            Columns[i] = new Column(columnWorker == SortColumn, (TextAnchor)columnWorker.CellStyle);
        }

        // Column labels
        var columnLabelsRow = new LabelsRow(Columns);

        for (int i = 0; i < columnsCount; i++)
        {
            var column = Columns[i];
            var columnWorker = columnWorkers[i];

            void drawSortIndicator(Rect rect)
            {
                if (SortColumn == columnWorker)
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
                    SortRowsByColumn(columnWorker);
                }
            }

            var columnDef = columnWorker.ColumnDef;

            columnLabelsRow[i] = columnDef
                .LabelFormat(columnDef, columnWorker.CellStyle)
                .PaddingAbs(cellPadHor, cellPadVer)
                .Background(drawSortIndicator)
                .ToButtonGhostly(
                    handleCellClick,
                    $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.Description}"
                );
        }

        HeaderRows.Add(columnLabelsRow);
        TotalHeaderRowsHeight += columnLabelsRow.Height;

        // Body rows
        foreach (var record in records)
        {
            var bodyRow = new BodyRow(Columns, record, this);

            for (int i = 0; i < columnsCount; i++)
            {
                var columnWorker = columnWorkers[i];

                try
                {
                    bodyRow[i] = columnWorker.GetTableCellWidget(record)
                        ?.PaddingAbs(cellPadHor, cellPadVer);
                }
                catch (Exception e)
                {
                    bodyRow[i] = new Label("!!!")
                        .Color(Color.red.ToTransparent(0.5f))
                        .TextAnchor(TextAnchor.LowerCenter)
                        .PaddingAbs(cellPadHor, cellPadVer)
                        .Tooltip(e.Message);
                }
            }

            BodyRows.Add(bodyRow);
            TotalBodyRowsHeight += bodyRow.Height;
        }

        // Filters
        var filtersRow = new Row(Columns);
        ActiveFilters = new(columnsCount);
        // Records enumerable can be a heavy generator.
        // In order to not execute it twice, we pass
        // body rows list as records to filter widget factory.
        records = BodyRows.Select(row => row.Object);

        for (int i = 0; i < columnsCount; i++)
        {
            var column = Columns[i];
            var columnWorker = columnWorkers[i];
            var filterWidget = columnWorker.GetFilterWidget(records);
            filterWidget.OnChange += filterWidget => HandleFilterChange(filterWidget, column);
            filtersRow[i] = filterWidget;
        }

        HeaderRows.Add(filtersRow);
        TotalHeaderRowsHeight += filtersRow.Height;

        // Finalize
        SortRowsByColumn(SortColumn);
    }

    private sealed class Column
    {
        public bool IsPinned;
        public float Width;
        public float InitialWidth;
        public readonly TextAnchor TextAnchor;
        public Column(bool isPinned, TextAnchor textAnchor)
        {
            IsPinned = isPinned;
            TextAnchor = textAnchor;
        }
    }
}
