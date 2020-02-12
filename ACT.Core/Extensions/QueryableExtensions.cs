using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class QueryableExtensions
    {
        public static IQueryable<string> SelectString<T>( this IQueryable<T> source, string propertyName )
        {
            var parameter = Expression.Parameter( typeof( T ), "x" );
            var selector = Expression.Lambda<Func<T, string>>(
                Expression.PropertyOrField( parameter, propertyName ),
                parameter );
            return source.Select( selector );
        }

        public static IQueryable<int> SelectInt<T>( this IQueryable<T> source, string propertyName )
        {
            var parameter = Expression.Parameter( typeof( T ), "x" );
            var selector = Expression.Lambda<Func<T, int>>(
                Expression.PropertyOrField( parameter, propertyName ),
                parameter );
            return source.Select( selector );
        }
    }
}
