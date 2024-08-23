using System;
using Verse;

namespace Stats;

public class Cell<ValueType> : IComparable<Cell<ValueType>> where ValueType : IComparable<ValueType>
{
    public ValueType value { get; }
    private readonly string valueStr;
    public Def? def { get; init; }
    public ThingDef? stuff { get; init; }
    public string? tip { get; init; }

    public Cell(ValueType value, string? valueStr = "")
    {
        this.value = value;

        if (!string.IsNullOrEmpty(valueStr))
        {
            this.valueStr = valueStr!;
        }
        else if (!string.IsNullOrEmpty(value?.ToString()))
        {
            this.valueStr = value!.ToString();
        }
        else
        {
            this.valueStr = valueStr!;
        }
    }

    public int CompareTo(Cell<ValueType> other)
    {
        return value.CompareTo(other.value);
    }

    public override string ToString()
    {
        return valueStr;
    }
}
