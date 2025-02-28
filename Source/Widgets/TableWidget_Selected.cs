using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class TableWidget_Selected : TableWidget_Base
{
    private readonly Dictionary<ColumnDef, float?> ColumnsMaxValues;
    public TableWidget_Selected(
        List<ColumnDef> columns,
        List<ColumnDef> columnsLeft,
        List<ColumnDef> columnsRight
    ) : base(columns)
    {
        Left = new TablePart_S(this, [.. columnsLeft]);
        Right = new TablePart_S(this, [.. columnsRight]);
        ColumnsMaxValues = new(Columns.Count);

        SyncLayout();
    }
    public void AddRow(ThingRec row)
    {
        RowsAll.Add(row);
        RowsCur.Add(row);
        ApplyFilters();
        SortRows();
        UpdateLayout(row);
        Left.SyncLayout();
        Right.SyncLayout();

        foreach (var column in Columns)
        {
            if (column.Worker is not IColumnWorker<float> columnWorker) continue;

            ColumnsMaxValues[column] = Math.Max(
                ColumnsMaxValues.TryGetValue(column) ?? 0f,
                columnWorker.GetValue(row)
            );
        }
    }
    public void RemoveRow(ThingRec row)
    {
        RowsAll.Remove(row);
        RowsCur.Remove(row);
        SyncLayout();
        ColumnsMaxValues.Clear();

        foreach (var row2 in RowsAll)
        {
            foreach (var column in Columns)
            {
                if (column.Worker is not IColumnWorker<float> columnWorker) continue;

                ColumnsMaxValues[column] = Math.Max(
                    ColumnsMaxValues.TryGetValue(column) ?? 0f,
                    columnWorker.GetValue(row2)
                );
            }
        }
    }
    protected override void HandleRowSelect(ThingRec row)
    {
        RemoveRow(row);
    }
    public bool Contains(ThingRec row)
    {
        return RowsAll.Contains(row);
    }
    private class TablePart_S : TablePart
    {
        public TablePart_S(TableWidget_Selected parent, List<ColumnDef> columns) :
            base(parent, columns)
        {
            ShouldDrawCellAddon = true;
        }
        protected override void DrawCellAddon(
            Rect cellRect,
            ColumnDef column,
            ThingRec row
        )
        {
            if (
                Event.current.type != EventType.Repaint
                || column.Worker is not IColumnWorker<float> columnWorker_Num
            ) return;

            var valueCell = columnWorker_Num.GetValue(row);

            if (valueCell == 0f) return;

            var valueMax = ((TableWidget_Selected)Parent)
                .ColumnsMaxValues[column] ?? 0f;

            if (valueMax == 0f) return;

            // I don't remember any negative stat values in the game,
            // but we are working not only with stats, so just in case.
            var valuePct = Math.Abs(valueCell / valueMax);

            Widgets.DrawLineHorizontal(
                cellRect.x + 5f,
                cellRect.yMax,
                (cellRect.width - 10f) * valuePct,
                column.isNegative
                    ? valuePct == 1f ? Color.red : Color.green
                    : valuePct == 1f ? Color.green : Color.red
            );
        }
    }
}
