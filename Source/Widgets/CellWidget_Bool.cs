using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Bool : ICellWidget
{
    public float MinWidth => TableWidget_Base.RowHeight;
    private readonly Texture2D Tex;
    public CellWidget_Bool(bool value)
    {
        Tex = Widgets.GetCheckboxTexture(value);
    }
    public void Draw(Rect targetRect)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
}
