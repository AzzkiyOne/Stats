using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private class Row
    {
        private readonly Column[] Columns;
        private readonly Widget?[] Cells;
        public float Height;
        public virtual bool IsVisible { get; protected set; } = true;
        public Row(Column[] columns)
        {
            Columns = columns;
            Cells = new Widget?[columns.Length];
        }
        public Widget? this[int i]
        {
            get => Cells[i];
            set
            {
                Cells[i] = value;

                if (value != null)
                {
                    var cellSize = value.GetSize();
                    var column = Columns[i];

                    if (column.Width < cellSize.x)
                    {
                        column.Width = column.InitialWidth = cellSize.x;
                    }

                    if (Height < cellSize.y)
                    {
                        Height = cellSize.y;
                    }
                }
            }
        }
        public virtual void Draw(
            Rect rect,
            float offsetX,
            bool drawPinned,
            float cellExtraWidth,
            int index
        )
        {
            var xMax = rect.width;
            rect.x = -offsetX;

            for (int i = 0; i < Cells.Length; i++)
            {
                if (rect.x >= xMax)
                {
                    break;
                }

                // It seems that this is faster than attaching column props object to a cell.
                var column = Columns[i];

                if (column.IsPinned != drawPinned)
                {
                    continue;
                }

                rect.width = column.Width + cellExtraWidth;
                if (rect.xMax > 0f)
                {
                    var cell = Cells[i];

                    if (cell != null)
                    {
                        var origTextAnchor = Text.Anchor;
                        Text.Anchor = column.TextAnchor;

                        try
                        {
                            // Basically, relative size extensions are not allowed on table cell widgets.
                            // Saves us some CPU cycles and is pointless to do anyway.
                            var cellSize = cell.GetSize();
                            rect.height = cellSize.y;

                            cell.Draw(rect, cellSize);
                        }
                        catch
                        {
                            // TODO: ?
                        }

                        Text.Anchor = origTextAnchor;
                    }
                }

                rect.x = rect.xMax;
            }
        }
    }

    private sealed class LabelsRow : Row
    {
        public LabelsRow(Column[] columns) : base(columns)
        {
        }
        public override void Draw(
            Rect rect,
            float offsetX,
            bool drawPinned,
            float cellExtraWidth,
            int index
        )
        {
            Verse.Widgets.DrawHighlight(rect);

            base.Draw(rect, offsetX, drawPinned, cellExtraWidth, index);
        }
    }

    private sealed class BodyRow : Row
    {
        public readonly TObject Object;
        private readonly ObjectTable<TObject> Parent;
        private bool IsHovered = false;
        private bool _IsHidden = false;
        public bool IsHidden
        {
            set
            {
                if (value == _IsHidden)
                {
                    return;
                }

                if (value == true)
                {
                    IsVisible = _IsPinned;
                }
                else
                {
                    IsVisible = true;
                }

                _IsHidden = value;
            }
        }
        private bool _IsPinned = false;
        private bool IsPinned
        {
            set
            {
                if (value == _IsPinned)
                {
                    return;
                }

                if (value == false)
                {
                    IsVisible = !_IsHidden;
                }
                //else
                //{
                //    IsVisible = true;
                //}

                _IsPinned = value;
            }
        }
        public override bool IsVisible
        {
            get => base.IsVisible;
            protected set
            {
                if (value == base.IsVisible)
                {
                    return;
                }

                if (value == false)
                {
                    Parent.TotalBodyRowsHeight -= Height;
                }
                else
                {
                    Parent.TotalBodyRowsHeight += Height;
                }

                base.IsVisible = value;
            }
        }
        public BodyRow(Column[] columns, TObject @object, ObjectTable<TObject> parent) : base(columns)
        {
            Object = @object;
            Parent = parent;
        }
        public override void Draw(
            Rect rect,
            float offsetX,
            bool drawPinned,
            float cellExtraWidth,
            int index
        )
        {
            var mouseIsOverRect = Mouse.IsOver(rect);

            if (Event.current.type == EventType.Repaint)
            {
                if (_IsPinned)
                {
                    Verse.Widgets.DrawHighlightSelected(rect);
                }

                if (mouseIsOverRect)
                {
                    IsHovered = true;
                }

                // IsHovered may be true even if mouse is not over rect.
                if (IsHovered)
                {
                    Verse.Widgets.DrawHighlight(rect);
                }
                else if (index % 2 == 0)
                {
                    Verse.Widgets.DrawLightHighlight(rect);
                }

                if (mouseIsOverRect == false)
                {
                    IsHovered = false;
                }
            }

            base.Draw(rect, offsetX, drawPinned, cellExtraWidth, index);

            // This must go after cells to not interfere with their GUI events.
            if
            (
                Event.current.control
                && Event.current.type == EventType.MouseDown
                && mouseIsOverRect
            )
            {
                IsPinned = !_IsPinned;
            }
        }
    }
}
