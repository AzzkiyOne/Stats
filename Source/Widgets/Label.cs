using UnityEngine;

namespace Stats.Widgets;

// Maybe text shouldn't be a widget, but rather just a text node with "ContentSize" prop?
// You can't set a width of a text. It must always have the width based on the text.
public sealed class Label
    : Widget
{
    protected override Vector2 Size { get; set; }
    private string? _Text;
    public string? Text
    {
        get => _Text;
        set
        {
            _Text = value;
            UpdateSize();
        }
    }
    public Label(string? text)
    {
        _Text = text;
        Size = GetSize();
    }
    public Label(string? text, out Label labelWidget)
        : this(text)
    {
        labelWidget = this;
    }
    public override Vector2 GetSize()
    {
        return Verse.Text.CalcSize(Text);
    }
    protected override void DrawContent(Rect rect)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        Verse.Widgets.Label(rect, Text);
    }
}
