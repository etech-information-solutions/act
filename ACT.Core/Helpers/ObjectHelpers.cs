using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Helpers
{
    public class ObjectHelpers
    {
        public object SetObjectProperty(string key, string value, object obj)
        {
            PropertyInfo propInfo = obj.GetType().GetProperties()
                                                 .FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (propInfo != null)
            {
                Type underlyingType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                if (string.IsNullOrEmpty(value))
                {
                    propInfo.SetValue(obj, null, null);
                }
                else
                {
                    object typedValue = Convert.ChangeType(value, underlyingType);
                    propInfo.SetValue(obj, typedValue, null);
                }

            }
            return obj;
        }
    }
}
