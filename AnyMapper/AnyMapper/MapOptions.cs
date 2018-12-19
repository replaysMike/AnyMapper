using System;

namespace AnyMapper
{
    /// <summary>
    /// Options for a mapping operation
    /// </summary>
    [Flags]
    public enum MapOptions
    {
        /// <summary>
        /// Map all properties
        /// </summary>
        None = 0,

        /// <summary>
        /// Entity Framework properties that are primary keys will be ignored
        /// </summary>
        IgnoreEntityKeys = 1,

        /// <summary>
        /// Entity Framework properties containing an auto-increment will be ignored
        /// </summary>
        IgnoreEntityAutoIncrementProperties = 2
    }
}
