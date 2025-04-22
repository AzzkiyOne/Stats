using System;
using Stats.Widgets.Extensions;
using Stats.Widgets.Extensions.Size;
using Stats.Widgets.Extensions.Size.Constraints;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Note to myself: Don't move this class into Stats.Widgets.Extensions namespace because
// it will then become required to import that namespace in order to use any widget
// extension, and widgets are almost always used with extensions. Also this could "leak"
// the abstraction.
public static class WidgetAPI
{
    // --- Size constraints: absolute ---

    public static AbsSizeWidgetExtension SizeAbs(
        this IWidget widget,
        float width,
        float height
    ) => new(
        widget,
        width,
        height
    );
    public static AbsSizeWidgetExtension SizeAbs(
        this IWidget widget,
        float size
    ) => widget.SizeAbs(
        size,
        size
    );
    public static AbsWidthWidgetExtension WidthAbs(
        this IWidget widget,
        float width
    ) => new(
        widget,
        width
    );
    public static AbsHeightWidgetExtension HeightAbs(
        this IWidget widget,
        float height
    ) => new(
        widget,
        height
    );

    // --- Size constraints: relative ---

    public static RelSizeWidgetExtension SizeRel(
        this IWidget widget,
        float parentWidthMultiplier,
        float parentHeightMultiplier
    ) => new(
        widget,
        parentWidthMultiplier,
        parentHeightMultiplier
    );
    public static RelSizeWidgetExtension SizeRel(
        this IWidget widget,
        float parentSizeMultiplier
    ) => widget.SizeRel(
        parentSizeMultiplier,
        parentSizeMultiplier
    );
    public static RelWidthWidgetExtension WidthRel(
        this IWidget widget,
        float parentWidthMultiplier
    ) => new(
        widget,
        parentWidthMultiplier
    );
    public static CalcWidthWidgetExtension WidthRel(
        this IWidget widget,
        SingleAxisSizeFunc widthFunction
    ) => new(
        widget,
        widthFunction
    );
    public static RelHeightWidgetExtension HeightRel(
        this IWidget widget,
        float parentHeightMultiplier
    ) => new(
        widget,
        parentHeightMultiplier
    );

    // --- Size modifiers: absolute ---

    public static AbsPaddingWidgetExtension PaddingAbs(
        this IWidget widget,
        float left,
        float right,
        float top,
        float bottom
    ) => new(
        widget,
        left,
        right,
        top,
        bottom
    );
    public static AbsPaddingWidgetExtension PaddingAbs(
        this IWidget widget,
        float horizontal,
        float vertical
    ) => widget.PaddingAbs(
        horizontal,
        horizontal,
        vertical,
        vertical
    );
    public static AbsPaddingWidgetExtension PaddingAbs(
        this IWidget widget,
        float amount
    ) => widget.PaddingAbs(
        amount,
        amount
    );

    // --- Size modifiers: relative ---

    public static RelPaddingWidgetExtension PaddingRel(
        this IWidget widget,
        float left,
        float right,
        float top,
        float bottom
    ) => new(
        widget,
        left,
        right,
        top,
        bottom
    );
    public static RelPaddingWidgetExtension PaddingRel(
        this IWidget widget,
        float horizontal,
        float vertical
    ) => widget.PaddingRel(
        horizontal,
        horizontal,
        vertical,
        vertical
    );
    public static RelPaddingWidgetExtension PaddingRel(
        this IWidget widget,
        float amount
    ) => widget.PaddingRel(
        amount,
        amount
    );

    // --- Misc ---

    public static BackgroundWidgetExtension Background(
        this IWidget widget,
        Texture2D texture
    ) => new(
        widget,
        texture
    );
    public static AlternatingBackgroundWidgetExtension Background(
        this IWidget widget,
        Texture2D idleTexture,
        Texture2D hoverTexture
    ) => new(
        widget,
        idleTexture,
        hoverTexture
    );
    public static DrawBackgroundWidgetExtension Background(
        this IWidget widget,
        Action<Rect> drawBackground
    ) => new(
        widget,
        drawBackground
    );
    public static HoverBackgroundWidgetExtension HoverBackground(
        this IWidget widget,
        Texture2D texture
    ) => new(
        widget,
        texture
    );
    public static ColorWidgetExtension Color(
        this IWidget widget,
        Color color
    ) => new(
        widget,
        color
    );
    public static ColorWidgetExtension Color(
        this IWidget widget,
        Color color,
        out ColorWidgetExtension colorExtension
    ) => colorExtension = widget.Color(
        color
    );
    public static HoverColorWidgetExtension HoverColor(
        this IWidget widget,
        Color color
    ) => new(
        widget,
        color
    );
    public static TextAnchorWidgetExtension TextAnchor(
        this IWidget widget,
        TextAnchor textAnchor
    ) => new(
        widget,
        textAnchor
    );
    public static TooltipWidgetExtension Tooltip(
        this IWidget widget,
        string text
    ) => new(
        widget,
        text
    );
    public static ClickEventWidgetExtension OnClick(
        this IWidget widget,
        Action action
    ) => new(
        widget,
        action
    );

    // --- Transformers ---

    public static IWidget ToButtonSubtle(
        this IWidget widget,
        Action clickEventHandler
    ) => widget
        .HoverBackground(TexUI.HighlightTex)
        .OnClick(clickEventHandler);
    public static IWidget ToButtonSubtle(
        this IWidget widget,
        Action clickEventHandler,
        string tooltip
    ) => widget
        .ToButtonSubtle(clickEventHandler)
        .Tooltip(tooltip);

    // --- Utils ---

    // Doesn't include widget's "own" size.
    public static Vector2 GetFixedSize(this IWidget widget)
    {
        return widget.GetSize(Vector2.zero);
    }
    public static void DrawIn(this IWidget widget, Rect rect)
    {
        var size = widget.GetSize(rect.size);
        widget.Draw(rect with { size = size }, rect.size);
    }
    public static T Get<T>(this IWidget widget) where T : IWidget
    {
        if (widget is T t)
        {
            return t;
        }
        else if (widget is WidgetDecorator widgetDecorator)
        {
            return widgetDecorator.Widget.Get<T>();
        }

        throw new Exception($"[{nameof(T)}] was not found!");
    }
}
