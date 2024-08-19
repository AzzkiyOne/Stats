using UnityEngine;

namespace Stats;

static class Rect_Extensions
{
    public static Rect CutFromX(this Rect rect, float x, float? amount = null)
    {
        return new Rect(x, rect.y, amount ?? rect.xMax - x, rect.height);
    }
    public static Rect CutFromX(this Rect rect, ref float x, float? amount = null)
    {
        var result = rect.CutFromX(x, amount);

        x = result.xMax;

        return result;
    }
    public static Rect CutFromY(this Rect rect, float y, float? amount = null)
    {
        return new Rect(rect.x, y, rect.width, amount ?? rect.yMax - y);
    }
    public static Rect CutFromY(this Rect rect, ref float y, float? amount = null)
    {
        var result = rect.CutFromY(y, amount);

        y = result.yMax;

        return result;
    }
}
