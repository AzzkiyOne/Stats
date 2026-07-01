using System.Collections.Generic;

namespace Stats.Tables;

public interface IRefRecordsProvider<T>
{
    public IEnumerable<T> Records { get; }
}
