using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class TechLevelColumnWorker<TRecord>(ColumnDef columnDef) :
    ColumnWorker<TRecord, TechLevelColumnWorker<TRecord>.TechLevelCell>(columnDef, ColumnType.String)
        where TRecord :
            IThingDefTableRecord
{
    protected override TechLevelCell MakeCell(TRecord record)
    {
        return new TechLevelCell(record.ThingDef.techLevel);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<TechLevel>> valueFieldFilterOptions = ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select<TechLevel, NTMFilterOption<TechLevel>>(
                techLevel => new(techLevel, techLevel.ToStringHuman().CapitalizeFirst())
            );
        Filter valueFieldFilter = new OTMFilter<TechLevel>((int row) => this[row].Value, valueFieldFilterOptions);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }

    public readonly struct TechLevelCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly TechLevel Value;

        private readonly string? _text;

        public TechLevelCell(TechLevel techLevel)
        {
            Value = techLevel;
            if (techLevel != TechLevel.Undefined)
            {
                _text = techLevel.ToStringHuman().CapitalizeFirst();
                Width = Text.CalcSize(_text).x;
            }
        }

        public void Draw(Rect rect)
        {
            if (Value != TechLevel.Undefined)
            {
                rect.Label(_text, GUIStyles.TableCell.String);
            }
        }
    }
}
