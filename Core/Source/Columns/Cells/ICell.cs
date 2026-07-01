using UnityEngine;

namespace Stats.Columns.Cells;

public interface ICell
{
    public float Width { get; }
    public bool IsRefreshable { get; }

    public void Draw(Rect rect);
}
