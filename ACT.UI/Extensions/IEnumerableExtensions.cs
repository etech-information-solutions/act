using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Reflection;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace System
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> AsSelectList<T, TKey>( this IEnumerable<T> list, Expression<Func<T, TKey>> valueExpression, Expression<Func<T, string>> textExpression )
        {
            return list.AsSelectList( valueExpression, textExpression, true, "Select..." );
        }

        public static IEnumerable<SelectListItem> AsSelectList<T, TKey>( this IEnumerable<T> list, Expression<Func<T, TKey>> valueExpression, Expression<Func<T, string>> textExpression, bool optional, string defaultText )
        {

            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> result = new List<SelectListItem>();

            if ( optional )
            {
                result.Add( new SelectListItem()
                {
                    Value = "",
                    Text = defaultText
                } );
            }

            if ( list != null )
            { //Extension methods gets called even if the "this" parameter is null.  In this case we want to handle it gracefully.

                Func<T, TKey> valueFunction = valueExpression.Compile();
                Func<T, string> textFunction = textExpression.Compile();

                //NB: It is assumed that the list is already sorted!
                foreach ( T item in list )
                {

                    //Try Catch?
                    string value = valueFunction.Invoke( item ).ToString();
                    string text = textFunction.Invoke( item ).ToString();

                    items.Add( new SelectListItem()
                    {
                        Value = value,
                        Text = text
                    } );

                }

            }


            result.AddRange( items.OrderBy( i => i.Text ) );
            return result;


        }


    }
}