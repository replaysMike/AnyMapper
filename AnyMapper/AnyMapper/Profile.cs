using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyMapper
{
    /// <summary>
    /// A mapping profile
    /// </summary>
    public class Profile
    {

        public Profile()
        {
            
        }

        public IMappingExpression<TSource, TDest> CreateMap<TSource, TDest>()
        {
            var expression = new MappingExpression<TSource, TDest>(this.GetType());

            return expression;
        }

        public TDest Resolve<TDest>(TDest value)
        {
            return value;
        }

        public TDest Resolve<TDest>(Func<TDest> func)
        {
            return func.Invoke();
        }

        internal ICollection<FieldMap> GetMappings()
        {
            var registry = MappingConfigurationResolutionContext.GetMappingRegistry();
            return registry.Mappings.Where(x => x.ProfileType == this.GetType()).ToList();
        }

        /// <summary>
        /// Asserts a valid configuration
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                return GetMappings().Count > 0;
            }
        }
    }
}
