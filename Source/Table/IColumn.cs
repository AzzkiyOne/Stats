using UnityEngine;

namespace Stats.Table;

public interface IColumn
{
    string Label { get; }
    string Description { get; }
    float MinWidth { get; }
    bool IsPinned { get; }
    TextAnchor TextAnchor { get; }
    bool ReverseDiffModeColors { get; }
    ICell? GetCell(ThingAlike thing);
}
