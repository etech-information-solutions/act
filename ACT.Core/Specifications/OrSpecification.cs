using System;
using System.Linq.Expressions;

namespace ACT.Core.Specifications
{
    public class OrSpecification<T> : CompositeSpecification<T>, ISpecification<T>
    {
        public OrSpecification( ISpecification<T> left, ISpecification<T> right ) : base( left, right ) { }

        public Expression<Func<T, bool>> GetExpression()
        {
            Expression<Func<T, bool>> expression = this.leftExp.Or( this.rightExp );

            return expression;
        }
    }
}
