using System;
using UnityEngine;

namespace Stats.Widgets;

// Highly experimental.
internal sealed class CompositeFilter<TObject> : FilterWidget<TObject>
{
    private readonly Widget Container;
    public CompositeFilter(Widget left, Widget right) : base(
        new CompositeExpression(
            left.Get<FilterWidget<TObject>>().Expression,
            right.Get<FilterWidget<TObject>>().Expression
        )
    )
    {
        Container = new HorizontalContainer([left, right], shareFreeSpace: true);
        Container.Parent = this;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Container.GetSize(containerSize);
    }
    protected override Vector2 CalcSize()
    {
        return Container.GetSize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Container.Draw(rect, containerSize);
    }
    public override FilterWidget<TObject> Clone()
    {
        // TODO: Maybe i don't need the whole cloning thing.
        throw new NotImplementedException();
    }

    private sealed class CompositeExpression : AbsExpression
    {
        public override bool IsEmpty => Left.IsEmpty && Right.IsEmpty;
        public override event Action<AbsExpression>? OnChange;
        private readonly AbsExpression Left;
        private readonly AbsExpression Right;
        public CompositeExpression(AbsExpression left, AbsExpression right)
        {
            Left = left;
            left.OnChange += expression => OnChange?.Invoke(this);
            Right = right;
            right.OnChange += expression => OnChange?.Invoke(this);
        }
        public override bool Eval(TObject @object)
        {
            // TODO: Maybe it will be a good idea to somehow pass table's filtering mode here
            // instead of hardcoding it to AND. But it may depend on the use case of the filter.
            return Left.Eval(@object) && Right.Eval(@object);
        }
        public override void NotifyChanged()
        {
            OnChange?.Invoke(this);
        }
        public override void Reset()
        {
            // TODO: This will cause double the amount of events to be emitted.
            Left.Reset();
            Right.Reset();
        }
    }
}
