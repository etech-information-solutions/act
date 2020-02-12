using System;
using System.Linq;
using System.Web;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static List<List<T>> Split<T>( this List<T> source, int size = 4000 )
        {
            return source
            .Select( ( x, i ) => new { Index = i, Value = x } )
            .GroupBy( x => x.Index / size )
            .Select( x => x.Select( v => v.Value ).ToList() )
            .ToList();
        }
    }
}