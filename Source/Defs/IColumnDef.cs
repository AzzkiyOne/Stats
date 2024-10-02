using UnityEngine;

namespace Stats;

internal interface IColumnDef
{
    string Label { get; }
    string Description { get; }
    float MinWidth { get; }
    bool IsPinned { get; }
    TextAnchor TextAnchor { get; }
    bool ReverseDiffModeColors { get; }
    ICellWidget? GetCellWidget(ThingAlike thing);
}
