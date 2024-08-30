using System;

namespace Stats;

// Implement Equals and arithmetic operators?
public readonly struct StrOrSingle : IComparable<StrOrSingle>
{
    public string Str { get; } = "";
    public float Single { get; } = float.NaN;
    public bool IsStr => !IsSingle;
    public bool IsSingle { get; }

    public StrOrSingle(string value) => (Str, Single, IsSingle) = (value, float.NaN, false);
    public StrOrSingle(float value) => (Str, Single, IsSingle) = ("", value, true);

    public override string ToString()
    {
        return IsStr ? Str : Single.ToString();
    }

    public int CompareTo(StrOrSingle other)
    {
        if (IsStr)
        {
            return Str.CompareTo(other.Str);
        }

        return Single.CompareTo(other.Single);
    }

    public static implicit operator StrOrSingle(string value) => new(value);
    public static implicit operator StrOrSingle(float value) => new(value);
}
