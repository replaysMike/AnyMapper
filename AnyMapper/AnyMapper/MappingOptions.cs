using System;

namespace AnyMapper
{
    /// <summary>
    /// The mapping options
    /// </summary>
    [Flags]
    public enum MappingOptions
    {
        /// <summary>
        /// No options specified
        /// </summary>
        None = 0,

        /// <summary>
        /// Specify if you want disable the ignore by attribute feature
        /// </summary>
        DisableIgnoreAttributes = 1,
    }
}
