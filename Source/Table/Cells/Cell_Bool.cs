using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

public sealed class Cell_Bool : ICell<bool>
{
    public bool Value { get; }
    private Texture2D Tex { get; }
    public Cell_Bool(bool value)
    {
        Value = value;
        Tex = Widgets.GetCheckboxTexture(value);
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor _)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
    public int CompareTo(ICell? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICell<bool>)other).Value);
    }
}
