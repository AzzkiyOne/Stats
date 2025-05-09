using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Stats.Widgets;

namespace Stats.ColumnWorkers;

public abstract class StatDrawEntryColumnWorker<TObject> : ColumnWorker<TObject>
{
    private static readonly Regex NonZeroNumberRegex = new(@"[1-9]{1}", RegexOptions.Compiled);
    private static readonly Regex NumberRegex = new(@"([0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private readonly Func<TObject, decimal> GetNumber;
    protected StatDrawEntryColumnWorker() : base(TableColumnCellStyle.Number)
    {
        GetNumber = new Func<TObject, decimal>(@object =>
        {
            var text = GetStatDrawEntryLabel(@object).Trim();

            if (text == "" || NonZeroNumberRegex.IsMatch(text) == false)
            {
                return 0m;
            }

            var match = NumberRegex.Match(text);

            return decimal.Parse(match.Groups[1].Captures[0].Value);
        }).Memoized();
    }
    protected abstract string GetStatDrawEntryLabel(TObject @object);
    public override Widget? GetTableCellWidget(TObject @object)
    {
        // Do not populate cache until we need to.
        var label = GetStatDrawEntryLabel(@object).Trim();

        if (label == "" || NonZeroNumberRegex.IsMatch(label) == false)
        {
            return null;
        }

        return new Label(label);
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> _)
    {
        return new NumberFilter<TObject>(GetNumber);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetNumber(object1).CompareTo(GetNumber(object2));
    }
}
