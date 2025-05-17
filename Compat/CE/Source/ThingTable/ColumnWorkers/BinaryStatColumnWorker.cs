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
        var label = base.GetStatDrawEntryLabel(thing);

        if (label.Length > 0)
        {
            label = label.Split(Separator).First();

            if (Utils.NonZeroNumberRegex.IsMatch(label))
            {
                return label.TrimEnd();
            }
        }

        return "";
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
        var label = base.GetStatDrawEntryLabel(thing);

        if (label.Length > 0)
        {
            label = label.Split(Separator).Last();

            if (Utils.NonZeroNumberRegex.IsMatch(label))
            {
                return label.TrimStart();
            }
        }

        return "";
    }
}
