using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Num : CellWidget_Base<float>
{
    public CellWidget_Num(float value, string text) : base(value, text)
    {
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect, Text);
        }
    }
}
