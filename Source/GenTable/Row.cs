using System;
using System.Collections.Generic;
using Verse;

namespace Stats.GenTable;

public class Row<DataType> : Dictionary<IColumn<DataType>, ICell?>
{
    public DataType Data { get; }
    public Row(DataType data, int size) : base(size)
    {
        Data = data;
    }
    public ICell? GetCell(IColumn<DataType> key, ICell? comparedTo = null)
    {
        var containsCell = TryGetValue(key, out var cell);

        if (!containsCell)
        {
            try
            {
                cell = key.GetCell(Data);

                if (
                    comparedTo is ICell<float> to
                    && cell is IAbleToBeDisplayedAsComparedTo dcell
                )
                {
                    dcell.DisplayAsComparedTo(to, key.ReverseDiffModeColors);
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"{key.Label}: {ex.Message}");
            }

            this[key] = cell;
        }

        return cell;
    }
}
