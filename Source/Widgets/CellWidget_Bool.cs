using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Bool : ICellWidget<bool>
{
    public bool Value { get; }
    public float MinWidth { get; } = TableWidget_Base.CellMinWidth;
    private readonly Texture2D Tex;
    public CellWidget_Bool(bool value)
    {
        Value = value;
        MinWidth += TableWidget_Base.RowHeight;
        Tex = Widgets.GetCheckboxTexture(Value);
    }
    public void Draw(Rect targetRect)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<bool>)other).Value);
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}
