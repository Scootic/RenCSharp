
using System;

namespace RenCSharp
{
    [Serializable]
    public enum ConditionalOperator
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanEquals,
        LessThanEquals,
        NotEqual
    }

    [Serializable]
    public enum ScreenShakeType
    {
        Horizontal,
        Vertical,
        BothAxis
    }
}
