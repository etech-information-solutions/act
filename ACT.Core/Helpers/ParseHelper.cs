using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ACT.Core.Helpers
{
    public static class ParseHelper
    {
        private static Dictionary<Type, MethodInfo> _typeParseMethodDictionary = new Dictionary<Type, MethodInfo>();
        private static Dictionary<Type, ConstructorInfo> _typeConstructorDictionary = new Dictionary<Type, ConstructorInfo>();
        private static List<Type> _unparsableTypes = new List<Type>();

        public static T Parse<T>( string value )
        {
            return ( T ) Parse( typeof( T ), value );
        }

        public static T Parse<T>( string value, T defaultValue )
        {
            try
            {
                return Parse<T>( value );
            }
            catch
            {
                return defaultValue;
            }
        }


        public static object Parse( Type type, string value )
        {

            Type useType = type;

            if ( type.IsGenericType && ( type.GetGenericTypeDefinition().Name.Contains( "Nullable" ) ) )
            {
                useType = Nullable.GetUnderlyingType( type );
            }

            if ( _unparsableTypes.Contains( useType ) )
            {
                //Don't go through the effort again!
                throw new Exception( string.Format( "Type {0} has no suiteable Parse method or Constructor.", useType.FullName ) );
            }


            if ( useType == typeof( string ) )
            {
                return value;
            }

            if ( useType.IsEnum )
            {
                return EnumHelper.Parse( useType, value );
            }

            if ( !_typeParseMethodDictionary.ContainsKey( useType ) && !_typeConstructorDictionary.ContainsKey( useType ) )
            {
                lock ( "ParseHelper" )
                {
                    try
                    {
                        //Try find Parse method:
                        MethodInfo method = useType.GetMethods()
                                                .FirstOrDefault( mi => ( mi.Name == "Parse" )
                                                            && ( mi.IsStatic )
                                                    //&& (mi.GetParameters().Count(p => p.ParameterType == typeof(string)) == 1));
                                                            && ( mi.GetParameters().Count() == 1 )
                                                            && ( mi.GetParameters().First().ParameterType == typeof( string ) ) );


                        if ( method != null )
                        {
                            _typeParseMethodDictionary.Add( useType, method );
                        }
                        else
                        {
                            //Try find Constructor:
                            ConstructorInfo ctor = useType.GetConstructors()
                                                        .FirstOrDefault( c => c.GetParameters().Count() == 1
                                                                            && c.GetParameters().First().ParameterType == typeof( string ) );

                            if ( ctor != null )
                            {
                                _typeConstructorDictionary.Add( useType, ctor );
                            }

                        }

                    }
                    catch ( ArgumentException )
                    {
                        //Assume ArgumentException implies another Thread already added the key.
                    }
                }
            }


            if ( _typeParseMethodDictionary.ContainsKey( useType ) )
            {
                MethodInfo parseMethod = _typeParseMethodDictionary[ useType ];
                //_log.Debug(string.Format("Calling {0}.Parse(\"{1}\")", useType.Name, value));
                try
                {
                    return parseMethod.Invoke( null, new object[] { value } );
                }
                catch ( Exception e )
                {
                    if ( e.InnerException != null )
                    {
                        throw ( e.InnerException );
                    }
                    throw ( e );
                }
            }

            if ( _typeConstructorDictionary.ContainsKey( useType ) )
            {
                ConstructorInfo constructor = _typeConstructorDictionary[ useType ];
                //_log.Debug(string.Format("Calling {0}.Ctor(\"{1}\")", useType.Name, value));
                try
                {
                    return constructor.Invoke( new object[] { value } );
                }
                catch ( Exception e )
                {
                    if ( e.InnerException != null )
                    {
                        throw ( e.InnerException );
                    }
                    throw ( e );
                }
            }


            _unparsableTypes.Add( useType );
            throw new Exception( string.Format( "Type {0} has no suiteable Parse method or Constructor.", useType.FullName ) );

        }


        public static object Parse( Type type, string value, object defaultValue )
        {

            try
            {
                return Parse( type, value );
            }
            catch
            {
                return defaultValue;
            }

        }

    }
}
