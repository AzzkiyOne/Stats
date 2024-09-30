using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

internal sealed class Cell_Bool : Cell<bool>
{
    private Texture2D Tex { get; }
    public Cell_Bool(bool value) : base(value, "")
    {
        Tex = Widgets.GetCheckboxTexture(Value);
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor _)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
}
