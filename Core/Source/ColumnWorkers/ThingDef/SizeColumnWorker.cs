using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class SizeColumnWorker<TRecord>(ColumnDef columnDef) :
    ColumnWorker<TRecord, SizeColumnWorker<TRecord>.SizeCell>(columnDef, ColumnType.Number)
        where TRecord :
            IThingDefTableRecord
{
    protected override SizeCell MakeCell(TRecord record)
    {
        IntVec2 size = GetSize(record.ThingDef);

        return new SizeCell(size);
    }

    private static IntVec2 GetSize(Verse.ThingDef thingDef)
    {
        IntVec2 size = thingDef.size;

        // Because 4x5=5x4.
        return new IntVec2(Math.Max(size.x, size.z), Math.Min(size.x, size.z));
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<decimal>> valueFieldFilterOptions = ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(GetSize)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<decimal>(size.Area, size.ToStringCross()));
        Filter valueFieldFilter = new OTMFilter<decimal>((int row) => this[row].Area, valueFieldFilterOptions);
        int Compare(int row1, int row2) => this[row1].Area.CompareTo(this[row2].Area);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }

    public readonly struct SizeCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly decimal Area;

        private readonly string? _text;

        public SizeCell(IntVec2 size)
        {
            Area = size.Area;
            if (Area != 0m)
            {
                _text = size.ToStringCross();
                Width = Text.CalcSize(_text).x;
            }
        }

        public void Draw(Rect rect)
        {
            if (_text != null)
            {
                rect.Label(_text, GUIStyles.TableCell.Number);
            }
        }
    }
}
