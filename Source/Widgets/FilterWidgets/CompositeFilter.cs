using System;
using UnityEngine;

namespace Stats.Widgets;

internal sealed class CompositeFilter<TObject> : FilterWidget<TObject>
{
    public override bool IsActive => Left.IsActive || Right.IsActive;
    public override event Action<FilterWidget<TObject>>? OnChange;
    private readonly FilterWidget<TObject> Left;
    private readonly FilterWidget<TObject> Right;
    private readonly Widget Container;
    public CompositeFilter(Widget left, Widget right)
    {
        Left = left.Get<FilterWidget<TObject>>();
        Left.OnChange += filter => OnChange?.Invoke(this);
        Right = right.Get<FilterWidget<TObject>>();
        Right.OnChange += filter => OnChange?.Invoke(this);
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
    public override bool Eval(TObject @object)
    {
        // TODO: Maybe it will be a good idea to somehow pass table's filtering mode here
        // instead of hardcoding it to AND. But it may depend on the use case of the filter.
        return Left.Eval(@object) && Right.Eval(@object);
    }
    public override void Reset()
    {
        // TODO: This will cause double the amount of events to be emitted.
        Left.Reset();
        Right.Reset();
    }
}
