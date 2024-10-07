using UnityEngine;
using Verse;

namespace Stats;

internal static class Debug
{
    public static void TryDrawUIDebugInfo(Rect targetRect, string text)
    {
        if (!InDebugMode)
        {
            return;
        }

        using (new GameFontCtx(GameFont.Small))
        using (new TextAnchorCtx(TextAnchor.MiddleCenter))
        {
            var textSize = Text.CalcSize(text);
            const float padding = 5f;
            var rectWidth = textSize.x + padding + 10f;
            var rectHeight = textSize.y + padding;
            var rect = new Rect(
                (targetRect.width - rectWidth) / 2,
                (targetRect.height - rectHeight) / 2,
                rectWidth,
                rectHeight
            );

            Widgets.DrawWindowBackground(rect);
            Widgets.Label(rect.ContractedBy(padding), text);
        }
    }
    public static bool InDebugMode => Event.current.alt;
}
