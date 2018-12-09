using System;
using System.Linq.Expressions;

namespace AnyMapper
{
    public class MappingExpression<TSource, TDest> : IMappingExpression<TSource, TDest>
    {
        public MappingExpression()
        {
        }

        public IMappingExpression<TSource, TDest> ForMember(Expression<Func<TSource, object>> destination, Expression<Func<TDest, object>> source)
        {
            return new MappingExpression<TSource, TDest>();
        }
    }
}
