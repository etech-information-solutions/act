using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ACT.Core.Attributes
{

    [AttributeUsage( AttributeTargets.Property )]
    public class FormattedDisplayNameAttribute : InstanceSpecificDisplayName
    {

        public FormattedDisplayNameAttribute( string format, params string[] properties )
        {
            this.Format = format;
            this.Properties = properties;
        }

        public string Format { get; set; }

        public string[] Properties { get; set; }

        public override string DisplayName
        {
            get
            {
                return this.Format;
            }
        }



        public override string GetDisplayName( object obj )
        {

            if ( obj == null )
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            List<object> propValues = new List<object>();

            foreach ( string propName in Properties )
            {
                PropertyInfo pi = type.GetProperty( propName );
                if ( pi != null )
                {

                    object propVal = string.Empty;
                    try
                    {
                        propVal = pi.GetValue( obj, null );
                    }
                    catch { }

                    propValues.Add( propVal );
                }
            }

            return string.Format( this.Format, propValues.ToArray() );


        }

    }
}
