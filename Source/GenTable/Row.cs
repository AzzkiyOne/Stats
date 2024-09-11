using System;
using System.Collections.Generic;
using Verse;

namespace Stats.GenTable;

public class Row<Key, DataType> :
    Dictionary<Key, Cell?>
    where Key : IRowKey<DataType>
{
    private DataType Data { get; }
    public Row(DataType data, int size) : base(size)
    {
        Data = data;
    }
    public Cell? GetCell(Key key, Cell? comparedTo = null)
    {
        var containsCell = TryGetValue(key, out var cell);

        if (!containsCell)
        {
            try
            {
                cell = key.Worker.GetCell(Data);

                if (comparedTo != null && cell is DiffableCell dcell)
                {
                    dcell.DisplayAsComparedTo(comparedTo);
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
    public ColumnWorker<DataType> Worker { get; }
}