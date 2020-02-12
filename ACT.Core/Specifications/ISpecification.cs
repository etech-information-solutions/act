using System;
using System.Linq.Expressions;

namespace ACT.Core.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> GetExpression();
    }
}
