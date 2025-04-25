using System;
using Stats.Widgets.Extensions;
using Stats.Widgets.Extensions.Background;
using Stats.Widgets.Extensions.Border;
using Stats.Widgets.Extensions.Color;
using Stats.Widgets.Extensions.Size;
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

    public static AbsSizeWidgetExtension
        SizeAbs(this Widget widget, float width, float height) =>
        new(widget, width, height);

    public static AbsSizeWidgetExtension
        SizeAbs(this Widget widget, float size) =>
        widget.SizeAbs(size, size);

    public static AbsWidthWidgetExtension
        WidthAbs(this Widget widget, float width) =>
        new(widget, width);

    public static AbsHeightWidgetExtension
        HeightAbs(this Widget widget, float height) =>
        new(widget, height);

    // --- Size constraints: relative ---

    public static RelSizeWidgetExtension
        SizeRel(this Widget widget, float parentWidthMultiplier, float parentHeightMultiplier) =>
        new(widget, parentWidthMultiplier, parentHeightMultiplier);

    public static RelSizeWidgetExtension
        SizeRel(this Widget widget, float parentSizeMultiplier) =>
        widget.SizeRel(parentSizeMultiplier, parentSizeMultiplier);

    public static RelWidthWidgetExtension
        WidthRel(this Widget widget, float parentWidthMultiplier) =>
        new(widget, parentWidthMultiplier);

    public static CalcWidthWidgetExtension
        WidthRel(this Widget widget, SingleAxisSizeFunc widthFunction) =>
        new(widget, widthFunction);

    public static RelHeightWidgetExtension
        HeightRel(this Widget widget, float parentHeightMultiplier) =>
        new(widget, parentHeightMultiplier);

    // --- Size modifiers: absolute ---

    public static AbsPaddingWidgetExtension
        PaddingAbs(this Widget widget, float left, float right, float top, float bottom) =>
        new(widget, left, right, top, bottom);

    public static AbsPaddingWidgetExtension
        PaddingAbs(this Widget widget, float horizontal, float vertical) =>
        widget.PaddingAbs(horizontal, horizontal, vertical, vertical);

    public static AbsPaddingWidgetExtension
        PaddingAbs(this Widget widget, float amount) =>
        widget.PaddingAbs(amount, amount);

    // --- Size modifiers: relative ---

    public static RelPaddingWidgetExtension
        PaddingRel(this Widget widget, float left, float right, float top, float bottom) =>
        new(widget, left, right, top, bottom);

    public static RelPaddingWidgetExtension
        PaddingRel(this Widget widget, float horizontal, float vertical) =>
        widget.PaddingRel(horizontal, horizontal, vertical, vertical);

    public static RelPaddingWidgetExtension
        PaddingRel(this Widget widget, float amount) =>
        widget.PaddingRel(amount, amount);

    // --- Background ---

    public static BackgroundWidgetExtension
        Background(this Widget widget, Texture2D texture, Color color) =>
        new(widget, texture, color);

    public static BackgroundWidgetExtension
        Background(this Widget widget, Texture2D texture) =>
        new(widget, texture, UnityEngine.Color.white);

    public static BackgroundWidgetExtension
        Background(this Widget widget, Color color) =>
        new(widget, BaseContent.WhiteTex, color);

    public static AlternatingBackgroundWidgetExtension
        Background(this Widget widget, Texture2D idleTexture, Texture2D hoverTexture) =>
        new(widget, idleTexture, hoverTexture);

    public static DrawBackgroundWidgetExtension
        Background(this Widget widget, Action<Rect> drawBackground) =>
        new(widget, drawBackground);

    public static HoverBackgroundWidgetExtension
        HoverBackground(this Widget widget, Texture2D texture, Color color) =>
        new(widget, texture, color);

    public static HoverBackgroundWidgetExtension
        HoverBackground(this Widget widget, Texture2D texture) =>
        widget.HoverBackground(texture, UnityEngine.Color.white);

    public static HoverBackgroundWidgetExtension
        HoverBackground(this Widget widget, Color color) =>
        widget.HoverBackground(BaseContent.WhiteTex, color);

    // --- Color ---

    public static ColorWidgetExtension
        Color(this Widget widget, Color color) =>
        new(widget, color);

    public static ColorWidgetExtension
        Color(this Widget widget, Color color, out ColorWidgetExtension colorExtension) =>
        colorExtension = widget.Color(color);

    public static HoverColorWidgetExtension
        HoverColor(this Widget widget, Color color) =>
        new(widget, color);

    // --- Borders ---

    public static BorderWidgetExtension
        Border(this Widget widget, float thickness, Color color) =>
        new(widget, thickness, color);

    public static BorderWidgetExtension
        Border(this Widget widget, Color color) =>
        widget.Border(1f, color);

    public static BorderWidgetExtension
        Border(this Widget widget, float thickness) =>
        widget.Border(thickness, UnityEngine.Color.white);

    public static BorderWidgetExtension
        Border(this Widget widget) =>
        widget.Border(1f, UnityEngine.Color.white);

    public static BorderBottomWidgetExtension
        BorderBottom(this Widget widget, float thickness, Color color) =>
        new(widget, thickness, color);

    public static BorderBottomWidgetExtension
        BorderBottom(this Widget widget, Color color) =>
        widget.BorderBottom(1f, color);

    public static BorderBottomWidgetExtension
        BorderBottom(this Widget widget, float thickness) =>
        widget.BorderBottom(thickness, UnityEngine.Color.white);

    public static BorderBottomWidgetExtension
        BorderBottom(this Widget widget) =>
        widget.BorderBottom(1f, UnityEngine.Color.white);

    // --- Misc ---

    public static TextAnchorWidgetExtension
        TextAnchor(this Widget widget, TextAnchor textAnchor) =>
        new(widget, textAnchor);

    public static TooltipWidgetExtension
        Tooltip(this Widget widget, string text) =>
        new(widget, text);

    public static ClickEventWidgetExtension
        OnClick(this Widget widget, Action action) =>
        new(widget, action);

    // --- Transformers ---

    public static Widget
        ToButtonSubtle(this Widget widget, Action clickEventHandler) =>
        widget.HoverBackground(TexUI.HighlightTex).OnClick(clickEventHandler);

    public static Widget
        ToButtonSubtle(this Widget widget, Action clickEventHandler, string tooltip) =>
        widget.ToButtonSubtle(clickEventHandler).Tooltip(tooltip);

    // --- Utils ---

    // Doesn't include widget's "own" size.
    public static Vector2 GetFixedSize(this Widget widget)
    {
        return widget.GetSize(Vector2.zero);
    }
    public static void DrawIn(this Widget widget, Rect rect)
    {
        var rectSize = rect.size;
        var size = widget.GetSize(rectSize);

        rect.size = size;

        widget.Draw(rect, rectSize);
    }
    public static T Get<T>(this Widget widget) where T : Widget
    {
        if (widget is T t)
        {
            return t;
        }
        else if (widget is WidgetExtension widgetExtension)
        {
            return widgetExtension.Widget.Get<T>();
        }

        throw new Exception($"[{nameof(T)}] was not found!");
    }
}
