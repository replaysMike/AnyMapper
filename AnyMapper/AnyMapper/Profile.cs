using System.Collections.Generic;
using System.Linq;

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
