using UnityEngine;

namespace Stats.Widgets;

internal interface ITableWidget
{
    void Draw(Rect rect);
    void ClearFilters();
}
