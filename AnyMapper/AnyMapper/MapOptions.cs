using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper
{
    /// <summary>
    /// Options for a mapping operation
    /// </summary>
    [Flags]
    public enum MapOptions
    {
        None = 0,
        IgnoreEntityKeys = 1,
    }
}
