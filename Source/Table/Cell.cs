using RimWorld;
using Verse;

namespace Stats;

public interface ICell
{
    public float? valueRaw { get; init; }
    public string? valueDisplay { get; init; }
    public string? valueExplanation { get; init; }
}

public class Cell : ICell
{
    public float? valueRaw { get; init; }
    public string? valueDisplay { get; init; }
    public string? valueExplanation { get; init; }
}

public class StatCell : Cell
{
    public StatCell(ThingDef thingDef, StatDef statDef)
    {
        // This is all very expensive.
        // The good thing is that it all will be cached.
        try
        {
            valueRaw = thingDef.GetStatValueAbstract(statDef);
        }
        catch
        {
        }

        if (valueRaw is float _valueRaw)
        {
            var statReq = StatRequest.For(thingDef, thingDef.defaultStuff);

            try
            {
                // Why ToStringNumberSense.Absolute?
                valueDisplay = statDef.Worker.GetStatDrawEntryLabel(statDef, _valueRaw, ToStringNumberSense.Absolute, statReq);
            }
            catch
            {
            }

            try
            {
                // Why valueRaw as final value?
                valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, _valueRaw);
            }
            catch
            {
            }
        }
    }
}