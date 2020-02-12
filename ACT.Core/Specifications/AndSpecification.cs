using System;
using System.Linq.Expressions;

namespace ACT.Core.Specifications
{
    public class AndSpecification<T> : CompositeSpecification<T>, ISpecification<T>
    {
        public AndSpecification( ISpecification<T> left, ISpecification<T> right ) : base( left, right ) { }

        public Expression<Func<T, bool>> GetExpression()
        {
            Expression<Func<T, bool>> expression = this.leftExp.And( this.rightExp );

            return expression;
        }
    }
}
