using UnityEngine;

namespace Stats;

public static class Globals
{
    public static class GUI
    {
        public const float Pad = 10f;
        public const float PadSm = 5f;
        public const float PadXs = 3f;
        public const float EstimatedInputFieldInnerPadding = 7f;
        public const float ButtonSubtleContentHoverOffset = 2f;
        public static readonly Color ActiveFilterOperatorColor = new(1f, 0.98f, 0.62f);
        public static float Opacity { get; set; } = 1f;
    }
}
