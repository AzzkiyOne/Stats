using System;

namespace Stats;

internal abstract class Widget_TableCell
    : Widget
{
    public Properties Props { get; }
    public Widget_TableCell(
        Properties props
    )
    {
        Props = props;
    }
    public class Properties
    {
        public bool IsPinned { get; set; } = false;
        private float _Width = 0f;
        public float Width
        {
            get => _Width;
            set => _Width = Math.Max(_Width, value);
        }
    }
}
