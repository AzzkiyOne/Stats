using System;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

// It maybe a good idea to have column workers return just "description" of a cell (in a form
// of a struct?). And then a row will create an appropriate cell.
// This way column workers will have to provide only bare minimum of cell's data required.
// Right now any cell's constructor requires a column instance and this is a constraint on a
// worker because we can't have workers that don't have a reference to its column.

public abstract class Cell : IComparable, IComparable<Cell>
{
    protected const float cellPadding = 5f;
    protected TextAnchor TextAnchor { get; }
    protected Cell(IColumn column)
    {
        TextAnchor = column.TextAnchor;
    }
    public abstract void Draw(Rect targetRect);
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
    public abstract int CompareTo(Cell? other);
}

public abstract class DiffableCell : Cell
{
    protected Color Color { get; set; } = Color.white;
    private bool ReverseDiffModeColors { get; }
    public DiffableCell(IColumn column) : base(column)
    {
        ReverseDiffModeColors = column.ReverseDiffModeColors;
    }
    public virtual void DisplayAsComparedTo(Cell other)
    {
        switch (CompareTo(other) * (ReverseDiffModeColors ? -1 : 1))
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
    }
}

public sealed class Cell_DefRef : Cell
{
    private string Text { get; }
    private string Value { get; }
    private string Tip { get; }
    private Def Def { get; }
    private ThingDef? Stuff { get; }
    public Cell_DefRef(IColumn column, Def def, ThingDef? stuff = null) : base(column)
    {
        Text = stuff != null ? def.LabelCap + " (" + stuff.LabelCap + ")" : def.LabelCap;
        Value = Text;
        Def = def;
        Stuff = stuff;
        Tip = def.description;
    }
    public override void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(cellPadding, 0);
        var currX = contentRect.x;

        // This is very expensive.
        Widgets.DefIcon(contentRect.CutFromX(ref currX, contentRect.height), Def, Stuff);

        currX += GenUI.Pad;

        if (Widgets.ButtonInvisible(targetRect))
        {
            GUIWidgets.DefInfoDialog(Def, Stuff);
        }

        using (new TextAnchorCtx(TextAnchor))
        {
            Widgets.Label(contentRect.CutFromX(ref currX), Text);
        }

        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
    public override int CompareTo(Cell? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is Cell_DefRef otherDefRefCell)
        {
            return Value.CompareTo(otherDefRefCell.Value);
        }

        throw new ArgumentException($"Argument must be {nameof(Cell_DefRef)}.");
    }
}

public sealed class Cell_Num : DiffableCell
{
    private string Text { get; set; }
    private float Value { get; }
    private string? Tip { get; set; }
    public Cell_Num(IColumn column, float value, string? value_str = null) : base(column)
    {
        Text = value_str ?? value.ToString();
        Value = value;
    }
    public override void Draw(Rect targetRect)
    {
        using (new ColorCtx(Color))
        using (new TextAnchorCtx(TextAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), Text);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
    public override int CompareTo(Cell? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is Cell_Num otherNumCell)
        {
            return Value.CompareTo(otherNumCell.Value);
        }

        throw new ArgumentException($"Argument must be {nameof(Cell_Num)}.");
    }
    public override void DisplayAsComparedTo(Cell other)
    {
        if (this == other)
        {
            return;
        }

        base.DisplayAsComparedTo(other);

        if (other is Cell_Num otherNumCell)
        {
            Tip = $"This: {Text}\n\nSelected: {otherNumCell.Text}";

            var diff = (Value / otherNumCell.Value);

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
    private string Value { get; }
    private string Tip { get; }
    public Cell_Str(IColumn column, string value, string tip = "") : base(column)
    {
        Value = value;
        Text = Value;
        Tip = tip;
    }
    public override void Draw(Rect targetRect)
    {
        using (new TextAnchorCtx(TextAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), Text);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
    public override int CompareTo(Cell? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is Cell_Str otherStrCell)
        {
            return Value.CompareTo(otherStrCell.Value);
        }

        throw new ArgumentException($"Argument must be {nameof(Cell_Str)}.");
    }
}

public sealed class Cell_Bool : Cell
{
    private string Text { get; }
    private bool Value { get; }
    public Cell_Bool(IColumn column, bool value) : base(column)
    {
        Value = value;
        Text = Value.ToString();
    }
    public Cell_Bool(IColumn column, float value) : base(column)
    {
        Value = value > 0f;
        Text = Value.ToString();
    }
    public override void Draw(Rect targetRect)
    {
        Widgets.DrawTextureFitted(targetRect, Widgets.GetCheckboxTexture(Value), 0.7f);
    }
    public override int CompareTo(Cell? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is Cell_Bool otherBoolCell)
        {
            return Value.CompareTo(otherBoolCell.Value);
        }

        throw new ArgumentException($"Argument must be {nameof(Cell_Bool)}.");
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