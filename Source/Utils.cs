using System.Text.RegularExpressions;

namespace Stats;

public static class Utils
{
    public static readonly Regex NonZeroNumberRegex = new(@"[1-9]{1}", RegexOptions.Compiled);
}
