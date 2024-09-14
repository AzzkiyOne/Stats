using System;
using System.Collections.Generic;
using Verse;

namespace Stats.GenTable;

public class Row<Key, DataType> :
    Dictionary<Key, ICell?>
    where Key : IRowKey<DataType>
{
    private DataType Data { get; }
    public Row(DataType data, int size) : base(size)
    {
        Data = data;
    }
    public ICell? GetCell(Key key, ICell? comparedTo = null)
    {
        var containsCell = TryGetValue(key, out var cell);

        if (!containsCell)
        {
            try
            {
                cell = key.Worker.GetCell(Data);

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
                Log.Warning(ex.Message);
            }

            this[key] = cell;
        }

        return cell;
    }
}

public interface IRowKey<DataType>
{
    public bool ReverseDiffModeColors { get; }
    public ColumnWorker<DataType> Worker { get; }
}