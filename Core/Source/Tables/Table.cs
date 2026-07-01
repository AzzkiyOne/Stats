using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Stats.Tables;

[Obsolete]
public abstract class Table
{
    internal abstract void Draw(Rect rect);

    internal abstract void NotifyParentWindowClosed();
}
