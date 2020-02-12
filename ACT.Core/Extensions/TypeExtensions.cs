using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel;

namespace System {
    public static class TypeExtensions {

        private static readonly Dictionary<Type, string[]> _typeDisplayNamesDictionary = new Dictionary<Type,string[]>();

        private static readonly Dictionary<Tuple<Type, string>, PropertyInfo> _typeDisplayNamePropertyInfoDictionary = new Dictionary<Tuple<Type,string>,PropertyInfo>();

        public static string[] GetDisplayNames(this Type type) {

            if (!_typeDisplayNamesDictionary.ContainsKey(type)) {
            
                lock ("foo") {
                
                    try {
                        
                        List<string> list = new List<string>();

                        foreach(PropertyInfo propertyInfo in type.GetProperties()) {
                            list.Add(propertyInfo.GetDisplayName());
                        }

                        _typeDisplayNamesDictionary.Add(type, list.ToArray());

                    } catch (ArgumentException) { }

                }

            }

            return _typeDisplayNamesDictionary[type];

        }

        public static PropertyInfo ResolveProperty(this Type type, string displayName) {

            Tuple<Type, string> key = Tuple.Create(type, displayName);

            if (!_typeDisplayNamePropertyInfoDictionary.ContainsKey(key)) {

                lock ("bar") {
                    try {

                        PropertyInfo propertyInfo = null;

                        //First we check for Property with DisplayName attribute.
                        propertyInfo = type.GetProperties().FirstOrDefault(pi => (pi.HasAttribute<DisplayNameAttribute>()) && (pi.GetAttribute<DisplayNameAttribute>().DisplayName == displayName));


                        //Then we look for Prorty
                        if (propertyInfo == null) {
                            propertyInfo = type.GetProperty(displayName.Replace(" ", ""));
                        }
                            
                        _typeDisplayNamePropertyInfoDictionary.Add(key, propertyInfo);
                        
                    } catch (ArgumentException) { }
                }

            }

            return _typeDisplayNamePropertyInfoDictionary[key];
        
        }


        public static PropertyInfo[] GetProperties(this Type type, bool flattenInterfaceHierarchy) {

            if ((!flattenInterfaceHierarchy) || (!type.IsInterface)) {
                return type.GetProperties();
            }

            List<PropertyInfo> properties = type.GetProperties().ToList();

            foreach (Type tt in type.GetInterfaces()) {

                PropertyInfo[] parentProperties = tt.GetProperties();
                properties.AddRange(
                    parentProperties
                        .Where(pp => !properties.Select(p => p.Name)
                                                .Contains(pp.Name))
                );

            }
            
            return properties.ToArray();
        }


        public static PropertyInfo GetProperty(this Type type, string name, bool flattenInterfaceHierarchy) {

            if ((!flattenInterfaceHierarchy) || (!type.IsInterface)) {
                return type.GetProperty(name);
            }

            return type.GetProperties(flattenInterfaceHierarchy).SingleOrDefault(pi => pi.Name == name);

        }

        public static PropertyInfo GetPropertyCaseInsensitive(this Type type, string name, bool flattenInterfaceHierarchy) {
            return type.GetProperties(flattenInterfaceHierarchy).SingleOrDefault(pi => pi.Name.ToLower() == name.ToLower());
        }

        public static bool Implements<TInterface>(this Type type) 
            /*where TInterface: class*/ {

            Type interfaceType = typeof(TInterface);
            return type.Implements(interfaceType);
        }

        public static bool Implements(this Type type, Type interfaceType) {
            if (!interfaceType.IsInterface) {
                throw new Exception(string.Format("{0} is not an interface.", interfaceType));
            }

            return type.GetInterfaces().Contains(interfaceType);
        }


        /// <summary>
        /// Gets all properties that returns T or a type that implements or inherits from T
        /// </summary>
        /// <typeparam name="TAssignable"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties<T>(this Type type, bool flattenInterfaceHierarchy) {

            List<PropertyInfo> result = new List<PropertyInfo>();

            Type tt = typeof(T);

            foreach (PropertyInfo pi in type.GetProperties(flattenInterfaceHierarchy)) {
                if (tt.IsInterface) {
                    if (pi.PropertyType.Implements<T>()) {
                        result.Add(pi);
                    }
                } else {
                    if (pi.PropertyType.IsAssignableFrom(tt)) {
                        result.Add(pi);
                    }
                }
            }

            return result.ToArray();

        }

    }



}