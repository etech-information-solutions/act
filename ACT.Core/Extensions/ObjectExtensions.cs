using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using ACT.Core.Helpers;

namespace System
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, TValue> ToDictionary<TValue>( this object obj )
        {

            //TODO: we can cache the fields and properties per type.
            Dictionary<string, TValue> result = new Dictionary<string, TValue>();

            if ( obj != null )
            {
                foreach ( PropertyInfo property in obj.GetType().GetProperties() )
                {

                    try
                    {

                        object value = property.GetValue( obj, null );
                        result.Add( property.Name, ParseHelper.Parse<TValue>( value.ToString() ) );

                    }
                    catch { } //Don't bother doing anything if a problem was encountered.

                }

            }

            return result;


        }

        public static IDictionary<string, object> ToDictionary( this object obj )
        {

            //TODO: we can cache the fields and properties per type.
            Dictionary<string, object> result = new Dictionary<string, object>();

            if ( obj != null )
            {
                foreach ( PropertyInfo property in obj.GetType().GetProperties() )
                {

                    try
                    {

                        result.Add( property.Name, property.GetValue( obj, null ) );

                    }
                    catch { } //Don't bother doing anything if a problem was encountered.

                }

            }

            return result;

        }

    }
}