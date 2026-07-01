using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Columns.Cells;
using Stats.Filters;
using Stats.TableRecords;
using Stats.Tables;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Columns.BuildableDef;

public class StatColumn<TRecord>(StatColumnDef columnDef) :
    Column<TRecord, StatColumn<TRecord>.StatCell>(columnDef, ColumnType.Number)
        where TRecord :
            IBuildableDefTableRecord
{
    private static readonly Regex _numberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    private readonly StatDef _stat = columnDef.stat;

    protected override StatCell MakeCell(TRecord record)
    {
        StatRequest statRequest = record.StatRequest;

        return MakeCell(statRequest);
    }

    protected virtual StatCell MakeCell(StatRequest statRequest)
    {
        if (_stat.Worker.ShouldShowFor(statRequest))
        {
            float statValue = _stat.Worker.GetValue(statRequest);

            return new StatCell(statValue, statRequest, _stat, this);
        }

        return default;
    }

    protected override StatCell RefreshCell(StatCell cell, out bool wasStale)
    {
        float newStatValue = _stat.Worker.GetValue(cell.StatRequest);

        if (newStatValue != cell.ValueRaw)
        {
            if (_cellTooltip != null && _cellTooltipOwner == cell.StatRequest)
            {
                _cellTooltip = null;
            }

            wasStale = true;
            return new StatCell(newStatValue, cell.StatRequest, _stat, this);
        }

        wasStale = false;
        return cell;
    }

    public override ICollection<CellField> GetCellFields(Table tableWorker)
    {
        Filter valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        CellField valueField = new(null, valueFieldFilter, Compare);

        return [valueField];
    }

    public override void NotifyRecordRemoved(int row)
    {
        _cellTooltipOwner = default;
        // TODO:
        //
        // Set _cellTooltip to null also?
        // Although cell removal code is already a bit heavier than i wanted it to be,
        // and i don't think a single string, that just hangs around in memory, is that big of an issue.

        base.NotifyRecordRemoved(row);
    }

    // TODO:
    //
    // Tooltip may become stale despite the cell's value being fresh.
    // This happens when we do not refresh a cell because its value haven't changed.
    // But the way we got to this value may have changed, and tooltip displays that info.
    private StatRequest _cellTooltipOwner;
    private TipSignal? _cellTooltip;

    public readonly struct StatCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => StatRequest.Thing != null;
        public readonly decimal Value;
        public readonly float ValueRaw;
        public readonly StatRequest StatRequest;

        private readonly StatDef? _stat;
        private readonly string? _text;
        private readonly StatColumn<TRecord> _column;

        public StatCell(float statValue, StatRequest statRequest, StatDef stat, StatColumn<TRecord> column)
        {
            _column = column;
            _stat = stat;
            StatRequest = statRequest;
            if (statValue != 0f)
            {
                ValueRaw = statValue;
                _text = stat.Worker.GetStatDrawEntryLabel(stat, statValue, _ToStringNumberSense, statRequest);
                Width = Text.CalcSize(_text).x;
                Match match = _numberRegex.Match(_text);
                if (match.Success)
                {
                    Value = decimal.Parse(match.Groups[1].Captures[0].Value);
                }
            }
        }

        public void Draw(Rect rect)
        {
            if (_text != null)
            {
                if (Mouse.IsOver(rect))
                {
                    HandleTooltip(rect);
                }

                rect.Label(_text, GUIStyles.TableCell.Number);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandleTooltip(Rect rect)
        {
            if (_column._cellTooltip == null || _column._cellTooltipOwner != StatRequest)
            {
                _column._cellTooltip = _stat.Worker.GetExplanationFull(StatRequest, _ToStringNumberSense, ValueRaw);
                _column._cellTooltipOwner = StatRequest;
            }
            rect.Tip(_column._cellTooltip.Value);
        }
    }
}
