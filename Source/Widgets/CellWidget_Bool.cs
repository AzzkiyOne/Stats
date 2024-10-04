using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Bool : CellWidget_Base<bool>
{
    private Texture2D Tex { get; }
    public CellWidget_Bool(bool value) : base(value, "")
    {
        Tex = Widgets.GetCheckboxTexture(Value);
        MinWidth = TableWidget.RowHeight;
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor _)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
    public override int CompareTo(ICellWidget<bool>? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
