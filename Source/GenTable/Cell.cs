using System;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

// It maybe a good idea to have column workers return just "description" of a cell (in a form
// of a struct?). And then a row will create an appropriate cell.
// This way column workers will have to provide only bare minimum of cell's data required.
//
// This will also allow for example to have a single column that displays leather amount and
// type (as an icon) but still filter by these 2 values separatedly.

public interface ICell : IComparable
{
    void Draw(Rect targetRect, TextAnchor textAnchor);
}

public interface ICell<T>
    where T : IComparable<T>
{
    T Value { get; }
}

public interface ICellWithText
{
    string Text { get; }
}

public interface IAbleToBeDisplayedAsComparedTo
{
    void DisplayAsComparedTo(ICell<float> other, bool reverseColors);
}

public abstract class Cell<T> : ICell, ICell<T>
     where T : IComparable<T>
{
    protected const float textPadding = 5f;
    public T Value { get; init; }
    public abstract void Draw(Rect targetRect, TextAnchor textAnchor);
    public int CompareTo(object? other)
    {
        if (other == null)
        {
            return 1;
        }
        else if (other is ICell<T> cell)
        {
            return Value.CompareTo(cell.Value);
        }

        throw new ArgumentException($"Argument must be {nameof(ICell<T>)}.");
    }
}

public sealed class Cell_DefRef : Cell<string>
{
    private Def Def { get; }
    private ThingDef? Stuff { get; }
    public Cell_DefRef(Def def, ThingDef? stuff = null)
    {
        // This is the only cell that calculates its own value.
        // Either this is wrong or other cells are wrong.
        Value = stuff != null ? $"{def.LabelCap} ({stuff.LabelCap})" : def.LabelCap;
        Def = def;
        Stuff = stuff;
    }
    public override void Draw(Rect targetRect, TextAnchor textAnchor)
    {
        var contentRect = targetRect.ContractedBy(textPadding, 0);
        var currX = contentRect.x;

        // This is very expensive.
        Widgets.DefIcon(contentRect.CutFromX(ref currX, contentRect.height), Def, Stuff);

        currX += GenUI.Pad;

        if (Widgets.ButtonInvisible(targetRect))
        {
            GUIWidgets.DefInfoDialog(Def, Stuff);
        }

        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect.CutFromX(ref currX), Value);
        }

        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, new TipSignal(Def.description));
    }
}

public sealed class Cell_Num : Cell<float>, ICellWithText, IAbleToBeDisplayedAsComparedTo
{
    public string Text { get; set; }
    private string Tip { get; set; } = "";
    private Color Color { get; set; } = Color.white;
    public Cell_Num(float value, string? value_str = null)
    {
        Value = value;
        Text = value_str ?? Value.ToString();
    }
    public override void Draw(Rect targetRect, TextAnchor textAnchor)
    {
        using (new ColorCtx(Color))
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(textPadding, 0), Text);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
    public void DisplayAsComparedTo(ICell<float> other, bool reverseColors)
    {
        if (this == other)
        {
            return;
        }

        switch (CompareTo(other) * (reverseColors ? -1 : 1))
        {
            case -1:
                Color = Color.red;
                break;
            case 1:
                Color = Color.green;
                break;
            case 0:
                Color = Color.yellow;
                break;
        }

        if (other is ICellWithText _other)
        {
            Tip = $"This: {Text}\n\nSelected: {_other.Text}";
        }

        var diff = Value / other.Value;

        if (float.IsFinite(diff))
        {
            Text = $"x {diff:0.0}";
        }
    }
}

public sealed class Cell_Str : Cell<string>
{
    private string Tip { get; }
    public Cell_Str(string value, string tip = "")
    {
        Value = value;
        Tip = tip;
    }
    public override void Draw(Rect targetRect, TextAnchor textAnchor)
    {
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(textPadding, 0), Value);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
}

public sealed class Cell_Bool : Cell<bool>
{
    private Texture2D Tex { get; }
    public Cell_Bool(bool value)
    {
        Value = value;
        Tex = Widgets.GetCheckboxTexture(Value);
    }
    public Cell_Bool(float value)
    {
        Value = value > 0f;
        Tex = Widgets.GetCheckboxTexture(Value);
    }
    public override void Draw(Rect targetRect, TextAnchor _)
    {
        Widgets.DrawTextureFitted(targetRect, Tex, 0.7f);
    }
}

//public sealed class Cell_Ex : Cell
//{
//    public Cell_Ex()
//    {
//        Text = "!!!";
//        TextAnchor = TextAnchor.MiddleCenter;
//        Tip = ex.ToString();
//        BGColor = Color.red;
//    }
//}