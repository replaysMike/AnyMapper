using System;

namespace AnyMapper
{
    /// <summary>
    /// The mapping options
    /// </summary>
    [Flags]
    public enum MappingSetupOptions
    {
        /// <summary>
        /// No options specified
        /// </summary>
        None = 0,

        /// <summary>
        /// Specify if you want disable the ignore by attribute feature
        /// </summary>
        DisableIgnoreAttributes = 1,

        /// <summary>
        /// Scan the current assembly for mapping profiles
        /// </summary>
        ScanCurrentAssembly = 2,

        /// <summary>
        /// Scan all assemblies for mapping profiles
        /// </summary>
        ScanAllAssemblies = 4
    }
}
