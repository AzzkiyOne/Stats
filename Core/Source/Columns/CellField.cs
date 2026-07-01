using System;
using Stats.Filters;

namespace Stats.Columns;

public readonly record struct CellField(string? Label, Filter? FilterWidget, Comparison<int>? Compare);
