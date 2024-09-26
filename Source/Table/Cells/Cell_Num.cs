using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

public sealed class Cell_Num : Cell<float>
{
    public Cell_Num(float value, string text) : base(value, text)
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
