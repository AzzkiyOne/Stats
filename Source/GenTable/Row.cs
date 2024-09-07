using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.GenTable;

public class Row<DataType> : Dictionary<IColumnDefWithWorker<DataType>, Cell?>
{
    private DataType Data { get; }

    public Row(DataType data)
    {
        Data = data;
    }

    public Cell? GetCell(IColumnDefWithWorker<DataType> column)
    {
        var containsCell = TryGetValue(column, out var cell);

        if (
            !containsCell
            && column.Worker.ShouldShowFor(Data)
        )
        {
            try
            {
                cell = new Cell
                {
                    Text = column.Worker.GetCellText(Data),
                    Tip = column.Worker.GetCellTip(Data),
                    TextAnchor = column.TextAnchor,
                    SortValue = column.Worker.GetCellSortValue(Data)
                };
            }
            catch (Exception ex)
            {
                //cell = new Cell
                //{
                //    Text = "!!!",
                //    TextAnchor = TextAnchor.MiddleCenter,
                //    Tip = ex.ToString(),
                //    BGColor = Color.red
                //};
            }
        }

        this[column] = cell;

        return cell;
    }
}