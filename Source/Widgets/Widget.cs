using System;
using UnityEngine;
using Verse;

namespace Stats;

// TODO:
// - Investigate functional composition instead.

// There are 2 cases in which a widget can be drawn.
//
// 1. We know the size of parent's content box.
// 
// In this case, a widget is drawn as usual.
//
// 2. We don't know the size of parent's content box.
//
// For example, we want to draw a popover, which contains our widget, but which is not
// bigger/smaller than the widget.
//
// First, we need to measure the widget. Same as we would measure text.
// Then, we pass the measured size to the widget and draw it as usual.

// Box model is implemented as "box-sizing: border-box".
public abstract class Widget
{
    public WidgetStyle Style { get; }
    public abstract Vector2 ContentSize { get; }
    private Vector2 CurContainerSize = Vector2.zero;
    private Vector2 CurMarginBoxSize = Vector2.zero;
    public Widget(WidgetStyle? style = null)
    {
        Style = style ?? WidgetStyle.Default;
    }
    public Vector2 GetMarginBoxSize(in Vector2 containerSize)
    {
        if
        (
            CurContainerSize.x == containerSize.x
            &&
            CurContainerSize.y == containerSize.y
        )
        {
            return CurMarginBoxSize;
        }

        CurContainerSize = containerSize;

        return CurMarginBoxSize = GetMarginBoxSize_Int(containerSize);
    }
    private Vector2 GetMarginBoxSize_Int(in Vector2 containerSize)
    {
        return Style switch
        {
            {
                Width: not null,
                Height: not null
            } => new Vector2(
                Style.Width.Get(containerSize.x),
                Style.Height.Get(containerSize.y)
            ),
            {
                Width: not null,
                Height: null
            } => new Vector2(
                Style.Width.Get(containerSize.x),
                ContentSize.y + Style.Padding.Ver
            ),
            {
                Width: null,
                Height: not null
            } => new Vector2(
                ContentSize.x + Style.Padding.Hor,
                Style.Height.Get(containerSize.y)
            ),
            _ => ContentSize + Style.Padding.Size,
        } + Style.Margin.Size;
    }
    public Vector2 GetMarginBoxSize()
    {
        return Style switch
        {
            {
                Width: WidgetStyle.Units.Abs absWidth,
                Height: WidgetStyle.Units.Abs absHeight
            } => new Vector2(
                absWidth.Value,
                absHeight.Value
            ),
            {
                Width: WidgetStyle.Units.Abs absWidth,
                Height: null
            } => new Vector2(
                absWidth.Value,
                ContentSize.y + Style.Padding.Ver
            ),
            {
                Width: null,
                Height: WidgetStyle.Units.Abs absHeight
            } => new Vector2(
                ContentSize.x + Style.Padding.Hor,
                absHeight.Value
            ),
            _ => ContentSize + Style.Padding.Size,
        } + Style.Margin.Size;
    }
    public void DrawIn(Rect container)
    {
        var marginBoxSize = GetMarginBoxSize(container.size);
        var marginBox = new Rect(container.position, marginBoxSize);

        DrawMarginBoxIn(marginBox, container);
    }
    public void DrawMarginBoxIn(Rect marginBox, Rect container)
    {
        Style.Align_H?.Invoke(ref container, ref marginBox);

        DrawMarginBox(marginBox);
    }
    public void DrawMarginBox(Rect marginBox)
    {
        // We can optimize here rendering in a scroll area.
        // If x/y coordinates are negative we can look if the margin box will be
        // visible.

        if (Mouse.IsOver(marginBox))
        {
            Widgets.DrawRectFast(marginBox, Color.cyan.ToTransparent(0.3f));
        }

        Style.Margin.ContractRect(ref marginBox);

        DrawBorderBox(marginBox);

        Style.Padding.ContractRect(ref marginBox);

        DrawContentBox(marginBox);
    }
    public virtual void DrawBorderBox(Rect borderBox)
    {
        Style.Background?.Invoke(borderBox, this);
    }
    public abstract void DrawContentBox(Rect contentBox);
}

public class WidgetStyle
{
    public Units.Unit? Width { get; init; } = 100;
    public Units.Unit? Height { get; init; }
    public BoxOffset Margin { get; init; } = 0f;
    public BoxOffset Padding { get; init; } = 0f;
    public AlignFunc? Align_H { get; init; }
    public Action<Rect, Widget>? Background { get; init; }
    public TextAnchor TextAlign { get; init; } = Constants.DefaultTextAnchor;
    // This is ok because style is readonly.
    internal static WidgetStyle Default { get; } = new WidgetStyle();

    public static class Units
    {
        public abstract class Unit
        {
            public abstract float Get(float value);
            public static implicit operator
                Unit(float v) => new Abs(v);
            public static implicit operator
                Unit(int v) => new Pct(v / 100f);
            public static implicit operator
                Unit(Func<float, float> v) => new Expr(v);
        }

        public sealed class Abs
            : Unit
        {
            public float Value { get; }
            public Abs(float value)
            {
                Value = value;
            }
            public override float Get(float _)
            {
                return Value;
            }
        }

        public sealed class Pct
            : Unit
        {
            private readonly float Mult;
            public Pct(float mult)
            {
                Mult = mult;
            }
            public override float Get(float value)
            {
                return value * Mult;
            }
        }

        public sealed class Expr
            : Unit
        {
            private readonly Func<float, float> Func;
            public Expr(Func<float, float> func)
            {
                Func = func;
            }
            public override float Get(float value)
            {
                return Math.Max(Func(value), 0f);
            }
        }
    }

    public delegate void AlignFunc(ref Rect outerRect, ref Rect innerRect);

    public static class Align
    {
        public static void Right(ref Rect outerRect, ref Rect innerRect)
        {
            innerRect.xMin = outerRect.xMax - innerRect.width;
            innerRect.xMax = outerRect.xMax;
        }
        public static void Middle_H(ref Rect outerRect, ref Rect innerRect)
        {
            var margin = (outerRect.width - innerRect.width) / 2f;

            innerRect.xMin += margin;
            innerRect.xMax = outerRect.xMax - margin;
        }
    }

    public class BoxOffset
    {
        public float Left { get; init; } = 0f;
        public float Right { get; init; } = 0f;
        public float Top { get; init; } = 0f;
        public float Bottom { get; init; } = 0f;
        public float Hor { get; }
        public float Ver { get; }
        public Vector2 Size { get; }
        public BoxOffset(float l, float r, float t, float b)
        {
            Left = l;
            Right = r;
            Top = t;
            Bottom = b;
            Hor = Left + Right;
            Ver = Top + Bottom;
            Size = new Vector2(Left + Right, Top + Bottom);
        }
        public BoxOffset(float hor, float ver)
            : this(hor, hor, ver, ver)
        {
        }
        public BoxOffset(float value)
            : this(value, value, value, value)
        {
        }
        public void ContractRect(ref Rect rect)
        {
            rect.x += Left;
            rect.y += Top;
            rect.width -= Hor;
            rect.height -= Ver;
        }
        public static implicit operator
            BoxOffset((float l, float r, float t, float b) v) => new(v.l, v.r, v.t, v.b);
        public static implicit operator
            BoxOffset((float hor, float ver) v) => new(v.hor, v.ver);
        public static implicit operator
            BoxOffset(float v) => new(v);
    }
}

//public class Box
//{
//    public WidgetStyle.Units.Unit? Width { get; init; }
//    public WidgetStyle.Units.Unit? Height { get; init; }
//    public WidgetStyle.BoxOffset Margin { get; init; } = 0f;
//    public WidgetStyle.BoxOffset Padding { get; init; } = 0f;
//}
