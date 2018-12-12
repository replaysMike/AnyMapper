using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeSupport.Extensions;

namespace AnyMapper
{
    public class MapperInstance
    {
        /// <summary>
        /// The mapping registry for mapping types
        /// </summary>
        public MappingRegistry Registry { get { return MappingConfigurationResolutionContext.GetMappingRegistry(); } }

        public MapperInstance()
        {

        }

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource source)
        {
            var provider = new MappingProvider();
            return provider.Map<TSource, TDest>(source);
        }

        /// <summary>
        /// Configure the mapper
        /// </summary>
        /// <param name="config"></param>
        public void Configure(Action<MappingConfiguration> config)
        {
            var configuration = new MappingConfiguration();
            config.Invoke(configuration);
        }

        /// <summary>
        /// Initialize the mapper and scan for profiles
        /// </summary>
        /// <param name="options"></param>
        public void Initialize(MappingOptions options = MappingOptions.ScanCurrentAssembly)
        {
            var type = typeof(Profile);
            if (options.BitwiseHasFlag(MappingOptions.ScanAllAssemblies))
            {
                // scan all known assemblies in the app domain
                Initialize(AppDomain.CurrentDomain.GetAssemblies());
            }
            else
            {
                // scan the current calling assembly for profiles
                Initialize(new Assembly[] { Assembly.GetCallingAssembly() });
            }
        }

        /// <summary>
        /// Initialize the mapper and scan for profiles within the assemblies passed
        /// </summary>
        /// <param name="assemblies"></param>
        public void Initialize(Assembly[] assemblies)
        {
            // scan a list of assemblies for profiles
            var type = typeof(AnyMapper.Profile);
            var profileTypes = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => type != p && type.IsAssignableFrom(p));

            var profiles = new List<Profile>();
            foreach (var profileType in profileTypes)
                profiles.Add((Profile)Activator.CreateInstance(profileType));

            Initialize(profiles.ToArray());
        }

        /// <summary>
        /// Initialize the mapper and use the profiles specified
        /// </summary>
        /// <param name="assemblies"></param>
        public void Initialize(params Profile[] profiles)
        {
            // add the profiles to the configuration
            Configure(config =>
            {
                config.AddProfiles(profiles);
            });
        }
    }
}
