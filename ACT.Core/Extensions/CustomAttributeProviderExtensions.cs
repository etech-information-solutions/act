using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace System.Reflection {
    public static class CustomAttributeProviderExtensions {

        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider)
            where T : class {

            //return provider.GetCustomAttributes(typeof(T), true).Cast<T>();
            //The above does not support attribute inheritance it seams

            return provider.GetCustomAttributes(true).OfType<T>();

        }

        public static T GetAttribute<T>(this ICustomAttributeProvider provider)
            where T : class {

            T[] attributes = provider.GetAttributes<T>().ToArray();
            return attributes.Length > 0 ? attributes[ 0 ] : null;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider provider)
            where T : class {

            return (provider.GetAttributes<T>().Any());
        }

        public static string GetDisplayName(this ICustomAttributeProvider provider) {

            if (provider.HasAttribute<DisplayNameAttribute>()) {
                return provider.GetAttribute<DisplayNameAttribute>().DisplayName;
            } else {

                string name = provider.ToString().Prettify();
                MemberInfo mi = provider as MemberInfo;
                if (mi != null) {
                    name = mi.Name.Prettify();
                }

                string[] arr = name.Split(' ');
                if ((arr.Length > 1) && (arr[arr.Length - 1].ToLower() == "id")) {
                    arr[arr.Length - 1] = "";
                }
                return arr.Delimit(" ", "").TrimEnd();
            }

        }

    }
}
