using System;
using System.Linq.Expressions;

namespace AnyMapper
{
    public class MappingExpression<TSource, TDest> : IMappingExpression<TSource, TDest>
    {
        public Type ProfileType { get; private set; }
        public Expression<Func<TDest, object>> Destination { get; private set; }
        public Expression<Func<TSource, object>> Source { get; private set; }
        public MappingContext Context { get; private set; }

        /// <summary>
        /// True if the type is registered explicitly
        /// </summary>
        public bool IsRegistered { get; set; }

        public MappingExpression(Type profileType)
        {
            ProfileType = profileType;
        }

        public MappingExpression()
        {
        }

        public IMappingExpression<TSource, TDest> ForMember(Expression<Func<TDest, object>> destination, Expression<Func<TSource, object>> source)
        {
            Source = source;
            Destination = destination;

            // resolve the current configuration and add it
            var registry = MappingConfigurationResolutionContext.GetMappingRegistry();
            registry.AddMapping(this);

            return new MappingExpression<TSource, TDest>(ProfileType);
        }

        public IMappingExpression<TSource, TDest> ForMember(Expression<Func<TDest, object>> destination, Func<TSource, MappingContext, Object> resolutionMethod)
        {
            Source = (x) => resolutionMethod.Invoke(x, Context);
            Destination = destination;

            // resolve the current configuration and add it
            var registry = MappingConfigurationResolutionContext.GetMappingRegistry();
            registry.AddMapping(this);

            return new MappingExpression<TSource, TDest>(ProfileType);
        }
    }
}
