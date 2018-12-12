using System.Collections.Generic;

namespace AnyMapper
{
    public class MappingConfiguration
    {
        /// <summary>
        /// Add multiple profiles to the configuration
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfiles(IEnumerable<Profile> profiles)
        {
            // resolve the current configuration and add it
            var registry = MappingConfigurationResolutionContext.GetMappingRegistry();
            foreach(var profile in profiles)
                registry.AddMapping(true, profile);
        }

        /// <summary>
        /// Add a profile to the configuration
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(Profile profile)
        {
            // resolve the current configuration and add it
            var registry = MappingConfigurationResolutionContext.GetMappingRegistry();
            registry.AddMapping(true, profile);
        }
    }
}
