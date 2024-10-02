using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Bool : CellWidget_Base<bool>
{
    private Texture2D Tex { get; }
    public CellWidget_Bool(bool value) : base(value, "")
    {
        Tex = Widgets.GetCheckboxTexture(Value);
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor _)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
}
