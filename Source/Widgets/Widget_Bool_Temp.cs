using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_Bool_Temp : IWidget
{
    public float MinWidth => Widget_Table.RowHeight;
    private readonly Texture2D Tex;
    public Widget_Bool_Temp(bool value)
    {
        Tex = Widgets.GetCheckboxTexture(value);
    }
    public void Draw(Rect targetRect)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
}
