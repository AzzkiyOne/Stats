using UnityEngine;

namespace Stats.Widgets;

public interface ITableWidget
{
    void Draw(Rect rect);
    void ClearFilters();
}
