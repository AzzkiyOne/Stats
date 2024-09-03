using UnityEngine;

namespace Stats;

public static class UnityEngine_Rect
{
    public static Rect CutFromX(this Rect rect, ref float x, float? amount = null)
    {
        var result = new Rect(x, rect.y, amount ?? rect.xMax - x, rect.height);

        x = result.xMax;

        return result;
    }
    public static Rect CutFromY(this Rect rect, ref float y, float? amount = null)
    {
        var result = new Rect(rect.x, y, rect.width, amount ?? rect.yMax - y);

        y = result.yMax;

        return result;
    }
}
