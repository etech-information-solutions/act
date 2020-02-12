using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using ACT.Core.Attributes;
using ACT.Core.Helpers;

namespace ACT.Core.Enums
{
    public static class EnumExtensions
    {

        private static Dictionary<Enum, string> _stringValuesDictionary = new Dictionary<Enum, string>();
        private static Dictionary<Enum, string> _displayTextDictionary = new Dictionary<Enum, string>();

        private static Dictionary<Enum, bool> _invalidDictionary = new Dictionary<Enum, bool>();
        private static Dictionary<Enum, bool> _ignoreDictionary = new Dictionary<Enum, bool>();

        private static Dictionary<Enum, FieldInfo> _fieldInfoDictionary = new Dictionary<Enum, FieldInfo>();

        /// <summary>
        /// Returns either the string value stored in the StringEnumValueAttribute or the
        /// string name of the field.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetStringValue( this Enum enumValue )
        {

            Type type = enumValue.GetType();

            if ( _stringValuesDictionary.ContainsKey( enumValue ) )
            {
                return _stringValuesDictionary[ enumValue ];
            }
            else
            {

                lock ( type )
                {

                    //FieldInfo fi = type.GetField(enumValue.ToString());
                    FieldInfo fi = enumValue.GetFieldInfo();
                    if ( fi != null )
                    {

                        foreach ( StringEnumValueAttribute attr in fi.GetCustomAttributes( typeof( StringEnumValueAttribute ), false ) )
                        {

                            try
                            {
                                _stringValuesDictionary.Add( enumValue, attr.Value );
                            }
                            catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                            return attr.Value;

                        }
                    }
                    else
                    {

                        //If we get here the value being parsed is not a valid option (i.e. it is numeic.
                        string def = ( ( Enum ) EnumHelper.GetDefault( type ) ).GetStringValue(); //NB: Recursion!
                        try
                        {
                            _stringValuesDictionary.Add( enumValue, def );
                        }
                        catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                        return def;

                    }


                    //If we made it to here this is a normal enum:
                    string val = enumValue.ToString();
                    try
                    {
                        _stringValuesDictionary.Add( enumValue, val );
                    }
                    catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                    return val;

                }
            }
        }


        public static string GetDisplayText( this Enum enumValue )
        {

            Type type = enumValue.GetType();

            if ( _displayTextDictionary.ContainsKey( enumValue ) )
            {
                return _displayTextDictionary[ enumValue ];
            }
            else
            {

                lock ( type )
                {

                    //FieldInfo fi = type.GetField(enumValue.ToString());
                    FieldInfo fi = enumValue.GetFieldInfo();
                    if ( fi != null )
                    {
                        foreach ( StringEnumDisplayTextAttribute attr in fi.GetCustomAttributes( typeof( StringEnumDisplayTextAttribute ), false ) )
                        {

                            try
                            {
                                _displayTextDictionary.Add( enumValue, attr.Value );
                            }
                            catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                            return attr.Value;

                        }
                    }
                    else
                    {
                        //If we get here the value being parsed is not a valid option (i.e. it is numeic.
                        string def = ( ( Enum ) EnumHelper.GetDefault( type ) ).GetDisplayText(); //NB: Recursion!
                        try
                        {
                            _displayTextDictionary.Add( enumValue, def );
                        }
                        catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                        return def;
                    }


                    //If we made it to here he do not have specific DisplayText
                    //string val = enumValue.GetStringValue().CamelToSpaced();
                    string val = enumValue.ToString().CamelToSpaced();
                    try
                    {
                        _displayTextDictionary.Add( enumValue, val );
                    }
                    catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                    return val;

                }
            }
        }


        public static bool IsInvalid( this Enum enumValue )
        {

            Type type = enumValue.GetType();


            if ( _invalidDictionary.ContainsKey( enumValue ) )
            {
                return _invalidDictionary[ enumValue ];
            }
            else
            {

                lock ( type )
                {

                    //FieldInfo fi = type.GetField(enumValue.ToString());
                    FieldInfo fi = enumValue.GetFieldInfo();
                    if ( fi != null )
                    {
                        if ( fi.GetCustomAttributes( typeof( InvalidEnumValueAttribute ), false ).Any() )
                        {

                            try
                            {
                                _invalidDictionary.Add( enumValue, true );
                            }
                            catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                            return true;

                        }
                    }
                    else
                    {
                        return true;
                    }

                    //If we made it to here the enum is allowed
                    try
                    {
                        _invalidDictionary.Add( enumValue, false );
                    }
                    catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                    return false;

                }
            }
        }


        public static bool UiIgnore( this Enum enumValue )
        {

            Type type = enumValue.GetType();


            if ( _ignoreDictionary.ContainsKey( enumValue ) )
            {
                return _ignoreDictionary[ enumValue ];
            }
            else
            {

                lock ( type )
                {

                    //FieldInfo fi = type.GetField(enumValue.ToString());
                    FieldInfo fi = enumValue.GetFieldInfo();
                    if ( fi != null )
                    {
                        if ( fi.GetCustomAttributes( typeof( UiIgnoreEnumValueAttribute ), false ).Any() )
                        {

                            try
                            {
                                _ignoreDictionary.Add( enumValue, true );
                            }
                            catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key

                            return true;

                        }
                    }
                    else
                    {
                        return true;
                    }

                    //If we made it to here the enum is allowed
                    try
                    {
                        _ignoreDictionary.Add( enumValue, false );
                    }
                    catch ( ArgumentException ) { } //Assume another Thread was trying to cache against the same key
                    return false;

                }
            }
        }

        /// <summary>
        /// Returns false if the underlying int value is 0 and true otherwise
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool GetBoolValue( this Enum enumValue )
        {
            return enumValue.GetIntValue() > 0;
        }


        /// <summary>
        /// Returns the underlying integer value.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static int GetIntValue( this Enum enumValue )
        {
            return Convert.ToInt32( enumValue );
        }


        /// <summary>
        /// Checks whether a particular enum value matches an enum flag filter.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool MatchesFilter<TEnum>( this TEnum enumValue, TEnum flagFilter ) where TEnum : struct
        {

            int enumInt = ( enumValue as Enum ).GetIntValue();
            int flagInt = ( flagFilter as Enum ).GetIntValue();

            return ( ( enumInt & flagInt ) != 0 );

        }



        public static FieldInfo GetFieldInfo( this Enum enumValue )
        {

            if ( !_fieldInfoDictionary.ContainsKey( enumValue ) )
            {

                Type type = enumValue.GetType();
                lock ( type )
                {

                    FieldInfo fi = type.GetField( enumValue.ToString() );
                    if ( fi != null )
                    {
                        try
                        {

                            _fieldInfoDictionary.Add( enumValue, fi );

                        }
                        catch ( ArgumentException ) { } //Assume this is due to another thread writing to the dictionary
                    }
                    else
                    {
                        return null; //don't cache
                    }

                }

            }

            return _fieldInfoDictionary[ enumValue ];

        }


    }
}
