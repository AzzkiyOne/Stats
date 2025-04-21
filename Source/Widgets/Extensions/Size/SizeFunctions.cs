using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public delegate float SingleAxisSizeFunc(in Vector2 containerSize);

public delegate Vector2 DoubleAxisSizeFunc(in Vector2 containerSize);
