using RimWorld;

namespace Stats.Table.Columns;

public class Column_Stat : Column_Num
{
    public StatDef stat;
    public bool formatValue = true;
    protected override float? GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (stat.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        return stat.Worker.GetValue(statReq);
    }
    protected override string FormatValue(float value)
    {
        if (formatValue == false)
        {
            return base.FormatValue(value);
        }

        return stat.Worker.ValueToString(value, true);
    }
    public override ICell? GetCell(ThingAlike thing)
    {
        if (Type == ColumnType.Boolean)
        {
            return GetValue(thing) is float value
                ? new Cells.Cell_Bool(value > 0f)
                : null;
        }

        return base.GetCell(thing);
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (string.IsNullOrEmpty(label))
        {
            label = stat.label;
        }

        if (string.IsNullOrEmpty(description))
        {
            description = stat.description;
        }
    }
}
