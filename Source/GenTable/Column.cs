using UnityEngine;

namespace Stats.GenTable;

public interface IColumn<DataType> : IRowKey<DataType>
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public bool IsPinned { get; }
    public TextAnchor TextAnchor { get; }
}
