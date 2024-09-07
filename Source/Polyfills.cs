// Probably better to install PolySharp.

#region init accessor

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

#endregion init accessor

#region required properties

namespace System.Runtime.CompilerServices
{
    public class RequiredMemberAttribute : Attribute { }

    public class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string name) { }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
}

#endregion required properties
