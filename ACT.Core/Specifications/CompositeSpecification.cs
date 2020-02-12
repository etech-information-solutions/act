using System;
using System.Linq.Expressions;

namespace ACT.Core.Specifications
{
    public abstract class CompositeSpecification<T>
    {
        protected Expression<Func<T, bool>> leftExp;
        protected Expression<Func<T, bool>> rightExp;

        protected CompositeSpecification( ISpecification<T> left, ISpecification<T> right )
        {
            this.leftExp = left.GetExpression();
            this.rightExp = right.GetExpression();
        }
    }
}