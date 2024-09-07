using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

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
                    SortValue = column.Worker.GetCellSortValue(Data),
                    DefRef = column.Worker.GetDefRef(Data),
                };
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                //cell = new Cell
                //{
                //    Text = "!!!",
                //    TextAnchor = TextAnchor.MiddleCenter,
                //    Tip = ex.ToString(),
                //    BGColor = Color.red,
                //};
            }
        }

        this[column] = cell;

        return cell;
    }
}
