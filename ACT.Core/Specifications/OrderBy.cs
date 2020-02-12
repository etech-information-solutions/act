using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ACT.Core.Specifications
{
    public class OrderBy<T, TProperty>
    {
        private readonly PropertyInfo propertyInfo;

        public OrderBy( Expression<Func<T, object>> expression )
        {
            UnaryExpression unaryExpression = ( UnaryExpression ) expression.Body;
            MemberExpression memberExpression = ( MemberExpression ) unaryExpression.Operand;

            this.propertyInfo = ( PropertyInfo ) memberExpression.Member;
        }

        public Expression<Func<T, TProperty>> GetExpression()
        {
            ParameterExpression instance = Expression.Parameter( typeof( T ), "instance" );
            MemberExpression property = Expression.Property( instance, this.propertyInfo );

            Expression<Func<T, TProperty>> expression = Expression.Lambda<Func<T, TProperty>>( property, instance );

            return expression;
        }
    }
}
