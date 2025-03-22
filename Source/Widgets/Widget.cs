using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

// TODO:
// - Investigate functional composition instead.
// - Some widgets are not containers. Maybe we can move all stuff related to containers
// in a subclass and simplify childless widgets.

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

// Essentially, we are trying to implement "box-sizing: border-box".
public abstract class Widget
{
    public string? Tooltip { get; set; }
    public AlignFunc? Align_H { get; set; }
    public Action<Rect, Widget>? Background { get; set; }
    public Units.Unit? Width { get; set; }
    public Units.Unit? Height { get; set; }
    public (float l, float r, float t, float b) Margin { get; set; } =
        (0f, 0f, 0f, 0f);
    public (float l, float r, float t, float b) Padding { get; set; } =
        (0f, 0f, 0f, 0f);
    private Vector2? _ChildrenBoundingBoxSize;
    private Vector2 ChildrenBoundingBoxSize
    {
        get
        {
            if (_ChildrenBoundingBoxSize is Vector2 cbbs) return cbbs;

            var xMax = 0f;
            var yMax = 0f;

            foreach (var childBox in GetLayout())
            {
                xMax = Math.Max(xMax, childBox.xMax);
                yMax = Math.Max(yMax, childBox.yMax);
            }

            return (Vector2)(_ChildrenBoundingBoxSize = new Vector2(xMax, yMax));
        }
    }
    protected List<Widget> Children { get; }
    public Widget(List<Widget> children)
    {
        Children = children;
    }
    private float ResolveBorderBoxWidth(float? parentContentBoxWidth)
    {
        if (Width is Units.Abs absWidth) return absWidth.Value;

        if (parentContentBoxWidth is float pcbw && Width != null) return Width.Get(pcbw);

        return ChildrenBoundingBoxSize.x + Padding.l + Padding.r;
    }
    private float ResolveBorderBoxHeight(float? parentContentBoxHeight)
    {
        if (Height is Units.Abs absHeight) return absHeight.Value;

        if (parentContentBoxHeight is float pcbh && Height != null) return Height.Get(pcbh);

        return ChildrenBoundingBoxSize.y + Padding.t + Padding.b;
    }
    public Vector2 GetMarginBoxSize(Vector2? parentContentBoxSize = null)
    {
        return new Vector2(
            ResolveBorderBoxWidth(parentContentBoxSize?.x) + Margin.l + Margin.r,
            ResolveBorderBoxHeight(parentContentBoxSize?.y) + Margin.t + Margin.b
        );
    }
    protected abstract IEnumerable<Rect> GetLayout(Vector2? contentBoxSize = null);
    public void DrawMarginBox(Rect marginBox)
    {
        // We can optimize here rendering in a scroll area.
        // If x/y coordinates are negative we can look if the margin box will be
        // visible.

        if (Mouse.IsOver(marginBox))
        {
            Widgets.DrawRectFast(marginBox, Color.cyan.ToTransparent(0.3f));
        }

        marginBox.ContractBy(Margin);

        if (Tooltip?.Length > 0)
        {
            TooltipHandler.TipRegion(marginBox, Tooltip);
        }

        Background?.Invoke(marginBox, this);

        marginBox.ContractBy(Padding);

        DrawContentBox(marginBox);
    }
    protected virtual void DrawContentBox(Rect contentBox)
    {
        var i = 0;

        foreach (var childMarginBox in GetLayout(contentBox.size))
        {
            var childWidget = Children[i];
            var childWidgetMarginBox = childMarginBox.OffsetBy(contentBox.position);

            childWidget.Align_H?.Invoke(ref contentBox, ref childWidgetMarginBox);

            childWidget.DrawMarginBox(childWidgetMarginBox);

            i++;
        }
    }
    public static class Units
    {
        public abstract class Unit
        {
            public abstract float Get(float value);
            public static implicit operator Unit(float v) => new Abs(v);
            public static implicit operator Unit(int v) => new Pct(v / 100f);
            //public static implicit operator Unit(Func<float, float> v) => new Expr(v);
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
}
