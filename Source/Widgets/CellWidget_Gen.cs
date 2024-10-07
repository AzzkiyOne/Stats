using System;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Gen<T> : ICellWidget<T> where T : notnull, IComparable<T>
{
    public T Value { get; }
    public float MinWidth { get; }
    private readonly string Text;
    private readonly string Tip;
    private readonly ThingIconWidget? Icon;
    private readonly ThingAlike? Thing;
    public CellWidget_Gen(
        T value,
        string text,
        string tip = "",
        ThingIconWidget? icon = null,
        ThingAlike? thing = null
    )
    {
        Value = value;
        MinWidth = Verse.Text.CalcSize(text).x;
        Text = text;
        Tip = tip;
        Icon = icon;
        Thing = thing;

        if (Icon != null)
        {
            MinWidth += TableWidget.RowHeight + TableWidget.CellPadding;
        }
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var curX = contentRect.x;

        if (Icon != null)
        {
            Icon.Draw(contentRect.CutFromX(ref curX, contentRect.height));
            curX += TableWidget.CellPadding;
        }

        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect.CutFromX(ref curX), Text);
        }

        if (Thing != null)
        {
            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                DefInfoDialogWidget.Draw(Thing.Def, Thing.Stuff);
            }
        }

        TooltipHandler.TipRegion(targetRect, Tip);
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<T>)other).Value);
    }
}
