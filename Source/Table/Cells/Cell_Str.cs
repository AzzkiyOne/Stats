using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

public sealed class Cell_Str : Cell<string>
{
    public Cell_Str(string value) : base(value, value)
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
