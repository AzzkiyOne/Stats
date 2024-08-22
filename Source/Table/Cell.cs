using System;
using UnityEngine;
using Verse;

namespace Stats;

public interface ICell : IComparable<ICell>
{
    public IComparable? value { get; }
}

public class Cell : ICell
{
    public IComparable? value { get; }
    private readonly string valueStr;
    public Def? def { get; init; }
    public ThingDef? stuff { get; init; }
    public string? tip { get; init; }

    public Cell(IComparable? value = null, string? valueStr = "")
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

    public int CompareTo(ICell other)
    {
        if (value is null)
        {
            if (other.value is null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (other.value is null)
            {
                return 1;
            }
            else
            {
                return value.CompareTo(other.value);
            }
        }
    }

    public override string ToString()
    {
        return valueStr;
    }

    public static readonly Cell Empty = new();
}
