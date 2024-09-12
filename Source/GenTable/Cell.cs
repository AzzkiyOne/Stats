using System;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

// It maybe a good idea to have column workers return just "description" of a cell (in a form
// of a struct?). And then a row will create an appropriate cell.
// This way column workers will have to provide only bare minimum of cell's data required.

public abstract class Cell : IComparable, IComparable<Cell>
{
    protected const float textPadding = 5f;
    public IComparable Value { get; init; }
    public abstract void Draw(Rect targetRect, TextAnchor textAnchor);
    public int CompareTo(object? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is Cell cell)
        {
            return CompareTo(cell);
        }

        throw new ArgumentException($"Argument must be {nameof(Cell)}.");
    }
    public int CompareTo(Cell? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}

public sealed class Cell_DefRef : Cell
{
    private string Text { get; }
    private string Tip { get; }
    private Def Def { get; }
    private ThingDef? Stuff { get; }
    public Cell_DefRef(Def def, ThingDef? stuff = null)
    {
        Text = stuff != null ? $"{def.LabelCap}({stuff.LabelCap})" : def.LabelCap;
        Value = Text;
        Def = def;
        Stuff = stuff;
        Tip = def.description;
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
            Widgets.Label(contentRect.CutFromX(ref currX), Text);
        }

        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
}

public sealed class Cell_Num : Cell
{
    private string Text { get; set; }
    private string Tip { get; set; }
    private Color Color { get; set; } = Color.white;
    public Cell_Num(float value, string? value_str = null)
    {
        Text = value_str ?? value.ToString();
        Value = value;
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
    public void DisplayAsComparedTo(Cell other, bool reverseColors)
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

        if (Value is float value && other is Cell_Num otherNumCell)
        {
            Tip = $"This: {Text}\n\nSelected: {otherNumCell.Text}";

            var diff = (value / (float)otherNumCell.Value);

            if (float.IsFinite(diff))
            {
                Text = $"x {diff:0.0}";
            }
        }
    }
}

public sealed class Cell_Str : Cell
{
    private string Text { get; }
    private string Tip { get; }
    public Cell_Str(string value, string tip = "")
    {
        Value = value;
        Text = Value.ToString();
        Tip = tip;
    }
    public override void Draw(Rect targetRect, TextAnchor textAnchor)
    {
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(textPadding, 0), Text);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
}

public sealed class Cell_Bool : Cell
{
    private Texture2D Tex { get; }
    public Cell_Bool(bool value)
    {
        Value = value;
        Tex = Widgets.GetCheckboxTexture(value);
    }
    public Cell_Bool(float value)
    {
        var value_bool = value > 0f;

        Value = value_bool;
        Tex = Widgets.GetCheckboxTexture(value_bool);
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