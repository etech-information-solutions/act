using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static class DictionaryExtensions
    {


        public static IDictionary<TKey, object> AddOrAppend<TKey>( this IDictionary<TKey, object> dictionary, TKey key, string value, string delimeter )
        {


            if ( dictionary.ContainsKey( key ) )
            {
                object val = dictionary[ key ];
                if ( val == null )
                {
                    dictionary[ key ] = delimeter + value;
                }
                else
                {
                    dictionary[ key ] = val.ToString() + delimeter + value;
                }

            }
            else
            {
                dictionary.Add( key, value );
            }

            return dictionary;
        }

        public static IDictionary<TKey, TValue> AddOrOverwrite<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, TValue value )
        {

            if ( dictionary.ContainsKey( key ) )
            {
                dictionary[ key ] = value;

            }
            else
            {
                dictionary.Add( key, value );
            }

            return dictionary;

        }

        public static string ToQueryString<TKey, TValue>( this IDictionary<TKey, TValue> dictionary )
        {

            if ( dictionary == null )
            {
                return string.Empty;
            }

            string s = "";
            bool first = true;
            foreach ( TKey key in dictionary.Keys )
            {
                if ( first )
                {
                    first = false;
                    s += "?";
                }
                else
                {
                    s += "&";
                }
                s += string.Format( "{0}={1}", key, dictionary[ key ] );
            }

            return s;

        }

        public static string ToHtmlAttributes<TKey, TValue>( this IDictionary<TKey, TValue> dictionary )
        {

            if ( dictionary == null )
            {
                return string.Empty;
            }

            string s = "";
            bool first = true;
            foreach ( TKey key in dictionary.Keys )
            {
                if ( first )
                {
                    first = false;
                }
                else
                {
                    s += " ";
                }
                s += string.Format( "{0}='{1}'", key, dictionary[ key ] );
            }

            return s;

        }


        public static IDictionary<string, string> LoadFrom( this IDictionary<string, string> dict, string source )
        {
            return dict.LoadFrom( source, '=', ';' );
        }

        public static IDictionary<string, string> LoadFrom( this IDictionary<string, string> dict, string source, char nameValueSplitter, char delimeter )
        {

            foreach ( string pair in source.Split( delimeter ) )
            {

                string[] keyVal = pair.Split( nameValueSplitter );
                if ( keyVal.Length >= 2 )
                {
                    dict.AddOrOverwrite( keyVal[ 0 ], keyVal[ 1 ] );
                }

            }

            return dict;
        }

    }
}