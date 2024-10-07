using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Str : ICellWidget<string>
{
    public string Value { get; }
    public float MinWidth { get; }
    public CellWidget_Str(string value)
    {
        Value = value;
        MinWidth = Text.CalcSize(value).x;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect, Value);
        }
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<string>)other).Value);
    }
}
