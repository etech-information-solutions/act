using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ACT.Core.Specifications
{
    public static class ExpressionExtensions
    {
        public static Expression<T> ComposeWith<T>( this Expression<T> left, Func<Expression, Expression, Expression> compose, Expression<T> right )
        {
            // build a Dictionary with the parameters from the right expression as keys
            Dictionary<ParameterExpression, ParameterExpression> parameterExpressions =
                left.Parameters.Select( ( lhs, i ) => new { lhs, rhs = right.Parameters[ i ] } )
                .ToDictionary( key => key.rhs, element => element.lhs );

            // replace parameters in the right expression with parameters from the left expression 
            Expression newBody = ParameterRewriter.ReplaceParameters( parameterExpressions, right.Body );

            Expression<T> composedExpression = Expression.Lambda<T>( compose( left.Body, newBody ), left.Parameters );

            return composedExpression;
        }

        public static Expression<Func<T, bool>> And<T>( this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right )
        {
            return left.ComposeWith( Expression.AndAlso, right );
        }

        public static Expression<Func<T, bool>> Or<T>( this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right )
        {
            return left.ComposeWith( Expression.Or, right );
        }
    }
}
