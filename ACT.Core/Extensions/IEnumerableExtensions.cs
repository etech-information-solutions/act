using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;

namespace System
{
    public static class IEnumerableExtensions
    {

        public static string Delimit(this IEnumerable enumerable)
        {
            return enumerable.Delimit(",", "");
        }

        public static string Delimit(this IEnumerable enumerable, char delimeter, char quote)
        {
            return enumerable.Delimit(delimeter.ToString(), quote.ToString());
        }

        public static string Delimit(this IEnumerable enumerable, string delimeter, string quote)
        {

            string result = "";

            if (enumerable == null)
            {
                return result;
            }

            bool first = true;

            foreach ( object o in enumerable)
            {

                if (first)
                {
                    first = false;
                }
                else
                {
                    result += delimeter;
                }

                result += o.ToString().Enquote(quote);

            }

            return result;

        }


        public static string Delimit<T>(this IEnumerable<T> enumerable, string delimeter, Func<T, string> conversion)
        {
            return enumerable.Delimit(delimeter, "", conversion);
        }

        public static string Delimit<T>(this IEnumerable<T> enumerable, string delimeter, string quote, Func<T, string> conversion)
        {
            string result = "";

            if (enumerable == null)
            {
                return result;
            }

            bool first = true;

            foreach (T o in enumerable)
            {

                if (first)
                {
                    first = false;
                }
                else
                {
                    result += delimeter;
                }

                result += conversion(o).Enquote(quote);

            }

            return result;
        }

        public static IEnumerable<T> RemoveRange<T>(this IEnumerable<T> enumerable, IEnumerable<T> removeObjects)
        {

            List<T> result = enumerable.ToList();

            foreach (T o in removeObjects)
            {
                if (result.Contains(o))
                {
                    result.Remove(o);
                }
            }

            return result;


        }


        public static void SortDynamic<T>(this List<T> list, string propertyName)
        {
            list.SortDynamic(propertyName, true);
        }

        public static void SortDynamic<T>(this List<T> list, string propertyName, bool ascending)
        {

            if (list == null) return;

            if (list.Count == 0) return;

            Type t = typeof(T);
            //PropertyInfo sortProperty = t.GetProperty(propertyName);
            PropertyInfo sortProperty = t.ResolveProperty(propertyName);

            if (sortProperty == null)
            {
                throw new Exception(string.Format("List<{0}> cannot be sorted by '{1}', because it is not a property of {0}.", t.FullName, propertyName));
            }

            list.Sort(delegate (T a, T b)
           {

               object aVal = sortProperty.GetValue(a, null);
               object bVal = sortProperty.GetValue(b, null);

               //what about Nullable<T>?
               IComparable aComp = aVal as IComparable;

               if ((aComp != null))
               {
                   try
                   {
                       int cc = aComp.CompareTo(bVal);
                       if (!ascending)
                       {
                           cc *= -1;
                       }
                       return cc;
                   }
                   catch { }
               }

               string aString = (aVal != null) ? aVal.ToString() : "";
               string bString = (bVal != null) ? bVal.ToString() : "";

               int ccc = aString.CompareTo(bString);
               if (!ascending)
               {
                   ccc *= -1;
               }

               return ccc;


           });


        }


        #region Nullable Max


        public static DateTime? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, DateTime?> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static DateTime? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, DateTime> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static int? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static int? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static long? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static long? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static decimal? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        public static decimal? NullableMax<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Max(selector);
        }

        #endregion


        #region NullableAny

        public static bool NullableAny<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return false;
            }
            else
            {
                return source.Any();
            }
        }

        public static bool NullableAny<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                return false;
            }
            else
            {
                return source.Any(predicate);
            }
        }

        #endregion

        #region NullableCount

        public static int NullableCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return 0;
            }
            else
            {
                return source.Count();
            }
        }

        public static int NullableCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                return 0;
            }
            else
            {
                return source.Count(predicate);
            }
        }


        #endregion


        #region NullableForEach

        public static void NullableForEach<T>(this List<T> list, Action<T> action)
        {
            if (list != null)
            {
                list.ForEach(action);
            }
        }

        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie)
            {
                action(e, i++);
            }
        }



        #endregion


        #region NullableAvg


        public static double? NullableAvg<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if ((source == null) || (!source.Any()))
            {
                return null;
            }

            return source.Average(selector);
        }

        //Other data types?


        #endregion


        #region NullableContains

        public static bool NullableContains<T>(this IEnumerable<T> _this, T value)
        {

            if (_this == null)
            {
                return false;
            }

            return _this.Contains(value);

        }


        #endregion


    }
}