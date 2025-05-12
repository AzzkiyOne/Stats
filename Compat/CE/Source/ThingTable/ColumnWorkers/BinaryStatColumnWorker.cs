using System.Linq;
using Stats.ThingTable;

namespace Stats.Compat.CE.ThingTable;

public sealed class BinaryStatColumnWorkerLeft : StatColumnWorker
{
    private readonly char Separator;
    public BinaryStatColumnWorkerLeft(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(Separator).First();
    }
}

public sealed class BinaryStatColumnWorkerRight : StatColumnWorker
{
    private readonly char Separator;
    public BinaryStatColumnWorkerRight(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(Separator).Last();
    }
}
