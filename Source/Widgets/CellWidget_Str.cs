using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Str : CellWidget_Base<string>
{
    public CellWidget_Str(string value) : base(value, value)
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
