using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Stats.Widgets;

namespace Stats;

// We do not sanitize labels here, because this will
// deoptimize cases where they don't need to be sanitized.
public abstract class StatDrawEntryColumnWorker<TObject> : ColumnWorker<TObject>
{
    private static readonly Regex NumberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private readonly Func<TObject, decimal> GetNumber;
    protected StatDrawEntryColumnWorker(ColumnDef columndef) : base(columndef, ColumnCellStyle.Number)
    {
        GetNumber = new Func<TObject, decimal>(@object =>
        {
            var label = GetStatDrawEntryLabel(@object);

            if (label.Length > 0)
            {
                var match = NumberRegex.Match(label);

                if (match.Success)
                {
                    return decimal.Parse(match.Groups[1].Captures[0].Value);
                }
            }

            return 0m;
        }).Memoized();
    }
    protected abstract string GetStatDrawEntryLabel(TObject @object);
    public override Widget? GetTableCellWidget(TObject @object)
    {
        // Do not populate cache until we need to.
        var label = GetStatDrawEntryLabel(@object);

        if (label.Length > 0)
        {
            return new Label(label);
        }

        return null;
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
