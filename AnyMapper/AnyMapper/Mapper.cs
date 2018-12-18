﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeSupport.Extensions;

namespace AnyMapper
{
    /// <summary>
    /// An instance of a mapper
    /// </summary>
    public class Mapper
    {
        private static readonly Mapper _instance = new Mapper();

        /// <summary>
        /// The mapping registry for mapping types
        /// </summary>
        public MappingRegistry Registry { get { return MappingConfigurationResolutionContext.GetMappingRegistry(); } }

        // don't mark type as BeforeFieldInit
        static Mapper()
        {
        }

        private Mapper()
        {
        }

        /// <summary>
        /// A mapping instance
        /// </summary>
        public static Mapper Instance
        {
            get
            {
                return _instance;
            }
        }

        #region Public methods

        #endregion

        #region Static methods

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest Map<TSource, TDest>(TSource source)
        {
            var provider = new MappingProvider();
            return provider.Map<TSource, TDest>(source);
        }

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static TDest Map<TSource, TDest>(TSource source, TDest dest)
        {
            var provider = new MappingProvider();
            return provider.Map<TSource, TDest>(source, dest);
        }

        /// <summary>
        /// Configure the mapper
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(Action<MappingConfiguration> config)
        {
            var configuration = new MappingConfiguration();
            config.Invoke(configuration);
        }

        /// <summary>
        /// Initialize the mapper and scan for profiles
        /// </summary>
        /// <param name="options"></param>
        public static void Initialize(MappingOptions options = MappingOptions.ScanCurrentAssembly)
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
        public static void Initialize(Assembly[] assemblies)
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
        public static void Initialize(params Profile[] profiles)
        {
            // add the profiles to the configuration
            Configure(config =>
            {
                config.AddProfiles(profiles);
            });
        }

        #endregion
    }
}
