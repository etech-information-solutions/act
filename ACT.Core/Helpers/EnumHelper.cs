using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ACT.Core.Attributes;
using ACT.Core.Enums;

namespace ACT.Core.Helpers
{
    public static class EnumHelper
    {

        /// <summary>
        /// Caches by Enum Type a dictionary of string values their corresponding Enum values.
        /// </summary>
        private static Dictionary<Type, Dictionary<string, Enum>> _enumTypeLookupDictionary = new Dictionary<Type, Dictionary<string, Enum>>();

        /// <summary>
        /// Caches by Enum Type the default value for the Enum Type.
        /// </summary>
        private static Dictionary<Type, Enum> _enumTypeDefaultValueDictionary = new Dictionary<Type, Enum>();


        /// <summary>
        /// Parses a string value as an Enum. If the enumType is decorated with the 
        /// StringEnumAttribute then it tries to find the correct field via the StringEnumValueAttribute.
        /// Otherwise it calls Enum.Parse.
        /// 
        /// Parse result gets cached in static dictionary for faster re-use.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Parse( Type enumType, string value )
        {
            return EnumHelper.Parse( enumType, value, false );
        }

        /// <summary>
        /// Parses a string value as an Enum. If the enumType is decorated with the 
        /// StringEnumAttribute then it tries to find the correct field via the StringEnumValueAttribute.
        /// Otherwise it calls Enum.Parse.
        /// 
        /// Parse result gets cached in static dictionary for faster re-use.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static object Parse( Type enumType, string value, bool ignoreCase )
        {

            if ( !enumType.IsEnum )
            {
                throw new InvalidCastException( string.Format( "The specified Type '{0}' is not an Enum", enumType.FullName ) );
            }

            //lock (enumType) { 

            if ( !_enumTypeLookupDictionary.ContainsKey( enumType ) )
            {

                lock ( enumType )
                {

                    try
                    {
                        _enumTypeLookupDictionary.Add( enumType, new Dictionary<string, Enum>() );
                    }
                    catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                }

            }

            if ( !_enumTypeLookupDictionary[ enumType ].ContainsKey( value ) )
            {

                lock ( enumType )
                {

                    bool found = false;
                    if ( enumType.GetCustomAttributes( typeof( StringEnumAttribute ), false ).Any() )
                    {

                        foreach ( FieldInfo fi in enumType.GetFields().Where( f => f.IsLiteral ) )
                        {

                            foreach ( StringEnumValueAttribute attr in fi.GetCustomAttributes( typeof( StringEnumValueAttribute ), false ) )
                            {

                                //Quirky logic that compiles both server side and Silverlight side
                                if ( ignoreCase )
                                {

                                    if ( string.Equals( attr.Value, value, StringComparison.CurrentCultureIgnoreCase ) )
                                    {
                                        found = true;

                                        try
                                        {
                                            _enumTypeLookupDictionary[ enumType ].Add( value, ( Enum ) fi.GetValue( null ) );
                                        }
                                        catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                                        break;
                                    }

                                }
                                else
                                {

                                    if ( string.Equals( attr.Value, value ) )
                                    {
                                        found = true;
                                        try
                                        {
                                            _enumTypeLookupDictionary[ enumType ].Add( value, ( Enum ) fi.GetValue( null ) );
                                        }
                                        catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                                        break;
                                    }

                                }

                            }
                            if ( found )
                            {
                                break;
                            }
                        }

                    }

                    if ( !found )
                    {
                        try
                        {
                            if ( string.IsNullOrEmpty( value ) )
                            {
                                //Enum.Parse will fail.  Revert to Default Value.
                                _enumTypeLookupDictionary[ enumType ].Add( value, ( Enum ) GetDefault( enumType ) );
                            }
                            else
                            {
                                _enumTypeLookupDictionary[ enumType ].Add( value, ( Enum ) Enum.Parse( enumType, value, ignoreCase ) );
                            }
                        }
                        catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                    }

                }
            }
            //}

            try
            {

                return _enumTypeLookupDictionary[ enumType ][ value ];

            }
            catch ( Exception )
            {

                //log?
                //throw new Exception(string.Format("Unable to parse value \"{0}\" against enum '{1}'.", value, enumType.FullName), e);
                return GetDefault( enumType );
            }


        }

        public static TEnum Parse<TEnum>( bool? value ) where TEnum : struct
        {
            return Parse( value, GetDefault<TEnum>() );
        }

        public static TEnum Parse<TEnum>( bool? value, TEnum defaultValue ) where TEnum : struct
        {
            if ( value.HasValue )
            {
                return Parse( value.Value, defaultValue );
            }
            else
            {
                return defaultValue;
            }
        }

        public static TEnum Parse<TEnum>( bool value ) where TEnum : struct
        {
            return Parse( value, GetDefault<TEnum>() );
        }

        public static TEnum Parse<TEnum>( bool value, TEnum defaultValue ) where TEnum : struct
        {
            if ( value )
            {
                return Parse( 1, defaultValue );
            }
            else
            {
                return Parse( 0, defaultValue );
            }
        }

        public static TEnum Parse<TEnum>( int? value ) where TEnum : struct
        {
            return Parse( value, GetDefault<TEnum>() );
        }

        public static TEnum Parse<TEnum>( int? value, TEnum defaultValue ) where TEnum : struct
        {
            if ( value.HasValue )
            {
                return Parse<TEnum>( value.Value );
            }
            else
            {
                return defaultValue;
            }
        }


        public static TEnum Parse<TEnum>( int value ) where TEnum : struct
        {
            return Parse( value, GetDefault<TEnum>() );
        }

        public static TEnum Parse<TEnum>( int value, TEnum defaultValue ) where TEnum : struct
        {
            return EnumHelper.Parse<TEnum>( value.ToString(), defaultValue );
        }

        public static TEnum Parse<TEnum>( string value ) where TEnum : struct
        {
            return EnumHelper.Parse<TEnum>( value, false );
        }

        public static TEnum Parse<TEnum>( string value, TEnum defaultValue ) where TEnum : struct
        {
            return EnumHelper.Parse<TEnum>( value, false, defaultValue );
        }

        public static TEnum Parse<TEnum>( string value, bool ignoreCase ) where TEnum : struct
        {
            return Parse( value, ignoreCase, GetDefault<TEnum>() );
        }

        public static TEnum Parse<TEnum>( string value, bool ignoreCase, TEnum defaultValue ) where TEnum : struct
        {

            try
            {

                return ( TEnum ) EnumHelper.Parse( typeof( TEnum ), value, ignoreCase );

            }
            catch
            { //something when wrong so lets return either the default or the first.

                //return EnumHelper.GetDefault<TEnum>();
                return defaultValue;

            }

        }

        public static IEnumerable<TEnum> GetOptions<TEnum>() where TEnum : struct
        {

            List<TEnum> list = new List<TEnum>();

            foreach ( FieldInfo field in typeof( TEnum ).GetFields().Where( f => f.IsLiteral ) )
            {
                list.Add( ( TEnum ) field.GetValue( null ) );
            }

            return list;

        }

        public static IEnumerable<object> GetOptions( Type enumType )
        {

            List<object> list = new List<object>();

            foreach ( FieldInfo field in enumType.GetFields().Where( f => f.IsLiteral ) )
            {
                list.Add( field.GetValue( null ) );
            }

            return list;

        }

        public static IEnumerable<TEnum> GetOptionsSortedAlphabetical<TEnum>() where TEnum : struct
        {
            return GetOptions<TEnum>().OrderBy( x => ( x as Enum ).GetStringValue() );
        }

        public static IEnumerable<TEnum> GetOptionsSortedNumeric<TEnum>() where TEnum : struct
        {
            return GetOptions<TEnum>().OrderBy( x => ( x as Enum ).GetIntValue() );
        }

        /// <summary>
        /// Gets the 1st field annotated with the [DefaultEnumValue] attribute or if none exists
        /// or simply the 1st field in the Enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum GetDefault<TEnum>() where TEnum : struct
        {
            return ( TEnum ) EnumHelper.GetDefault( typeof( TEnum ) );
        }

        /// <summary>
        /// Gets the 1st field annotated with the [DefaultEnumValue] attribute or if none exists
        /// or simply the 1st field in the Enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object GetDefault( Type enumType )
        {

            if ( !_enumTypeDefaultValueDictionary.ContainsKey( enumType ) )
            {

                List<FieldInfo> fields = enumType
                                        .GetFields()
                                        .Where( f => f.IsLiteral )
                                        .ToList();


                if ( fields.Count <= 0 )
                {
                    throw new Exception( string.Format( "No literal fields found on Type: '{0}'", enumType.Name ) );
                }

                //Look for anotated:
                lock ( enumType )
                {

                    FieldInfo fieldInfo = null;

                    if ( fields.Any( f => f.GetCustomAttributes( typeof( DefaultEnumValueAttribute ), true ).Any() ) )
                    {

                        fieldInfo = fields.First( f => f.GetCustomAttributes( typeof( DefaultEnumValueAttribute ), true ).Any() );

                    }
                    else
                    {

                        fieldInfo = fields.First();

                    }

                    try
                    {
                        _enumTypeDefaultValueDictionary.Add( enumType, ( Enum ) EnumHelper.Parse( enumType, fieldInfo.GetRawConstantValue().ToString() ) );
                    }
                    catch ( ArgumentException ) { } //Assume another thread was also inserting?

                }

            }

            return _enumTypeDefaultValueDictionary[ enumType ];


        }

        /// <summary>
        /// Sets all Enum properties to default values.
        /// </summary>
        /// <param name="target"></param>
        public static void InitEnumProperties( object target )
        {

            foreach ( PropertyInfo propertyInfo in target.GetType().GetProperties().Where( pi => pi.PropertyType.IsEnum ) )
            {
                try
                {
                    propertyInfo.SetValue( target, EnumHelper.GetDefault( propertyInfo.PropertyType ), null );
                }
                catch { }
            }

        }



    }
}
