using System;
using UnityEngine;

namespace Stats.Widgets;

public interface ITableWidget
{
    void Draw(Rect rect);
    void ResetFilters();
    TableFilterMode FilterMode { get; set; }
    void ToggleFilterMode();
    event Action<TableFilterMode> OnFilterModeChange;
}

public enum TableFilterMode
{
    AND = 0,
    OR = 1,
}
