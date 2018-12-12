namespace AnyMapper
{
    /// <summary>
    /// Resolves a mapping registry from the ambient context
    /// </summary>
    public static class MappingConfigurationResolutionContext
    {
        private const string RegistryName = "MappingRegistry";

        public static MappingRegistry GetMappingRegistry()
        {
            var registry = CallContext<MappingRegistry>.GetData(RegistryName);
            if (registry == null)
            {
                registry = new MappingRegistry();
                CallContext<MappingRegistry>.SetData(RegistryName, registry);
            }
            return registry;
        }
    }
}
