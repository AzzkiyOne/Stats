using UnityEngine;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly Column[] Columns;

    private sealed class Column
    {
        public bool IsPinned;
        public float Width;
        public float InitialWidth;
        public readonly TextAnchor TextAnchor;
        public readonly ColumnWorker<TObject> Worker;
        public Column(ColumnWorker<TObject> worker, bool isPinned)
        {
            Worker = worker;
            IsPinned = isPinned;
            TextAnchor = (TextAnchor)worker.CellStyle;
        }
    }
}
