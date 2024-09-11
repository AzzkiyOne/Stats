using UnityEngine;

namespace Stats.GenTable;

public interface IColumn
{
    public string Label { get; }
    public string Description { get; }
    public float MinWidth { get; }
    public TextAnchor TextAnchor { get; }
    public bool IsPinned { get; }
    public bool ReverseDiffModeColors { get; }
}
