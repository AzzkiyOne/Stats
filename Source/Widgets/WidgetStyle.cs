using System;
using UnityEngine;

namespace Stats;

public class WidgetStyle
{
    public Units.Unit? Width { get; init; } = 100;
    public Units.Unit? Height { get; init; }
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
}
