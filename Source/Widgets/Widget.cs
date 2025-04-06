using System.Diagnostics;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget
    : IWidget
{
    protected Vector2 Size { get; set; } = Vector2.zero;
    public bool WidthIsUndef { protected get; set; } = true;
    public bool HeightIsUndef { protected get; set; } = true;
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return Size;
    }
    public void Draw(Rect rect, in Vector2 containerSize)
    {
        DrawDebugInfo(rect);
        DrawContent(rect);
    }
    protected abstract void DrawContent(Rect rect);
    [Conditional("DEBUG")]
    private void DrawDebugInfo(Rect rect)
    {
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawRectFast(rect, Color.cyan.ToTransparent(0.3f));
            //WriteDebugLine(this, $"{rect.width} x {rect.height}");
            //DebugInfoLines.Reverse();
            //TooltipHandler.TipRegion(rect, string.Join("\n", DebugInfoLines));
            TooltipHandler.TipRegion(rect, $"{GetType().Name}: {rect.width} x {rect.height}");
        }

        //DebugInfoLines.Clear();
    }
    //[Conditional("DEBUG")]
    //public static void WriteDebugLine(IWidget widget, string info)
    //{
    //    DebugInfoLines.Add($"{widget.GetType().Name}: {info}");
    //}
    //private static readonly List<string> DebugInfoLines = new(20);
}
