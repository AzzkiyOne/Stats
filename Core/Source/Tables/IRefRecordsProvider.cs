using System;
using System.Collections.Generic;

namespace Stats.Tables;

[Obsolete]// ?
public interface IRefRecordsProvider<T>
{
    public IEnumerable<T> Records { get; }
}
