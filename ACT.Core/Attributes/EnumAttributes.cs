using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACT.Core.Attributes
{
    /// <summary>
    /// Indicates that a specific field is the default option in an Enum.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class DefaultEnumValueAttribute : Attribute
    {
    }

    /// <summary>
    /// Can be used to render an emply value in a Drop down or Radio for client side validation purposes
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class InvalidEnumValueAttribute : Attribute
    {
    }


    /// <summary>
    /// Can be used force UI rendering (Radio buttons or DropDowns) to not render
    /// options for a partifular field.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class UiIgnoreEnumValueAttribute : Attribute
    {
    }


    /// <summary>
    /// Attribute to show that Enum should be interpreted as string via StringEnumValueAttribute.
    /// Got the basic idea from http://www.codeproject.com/KB/cs/stringenum.aspx
    /// </summary>
    [AttributeUsage( AttributeTargets.Enum, AllowMultiple = false )]
    public class StringEnumAttribute : Attribute
    {

    }


    /// <summary>
    /// Holder to store a string value associated with a field on an enum.
    /// Got the basic idea from http://www.codeproject.com/KB/cs/stringenum.aspx
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class StringEnumValueAttribute : Attribute
    {

        public StringEnumValueAttribute( string value )
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    /// <summary>
    /// Optionally store a display value alternative to the StringEnumValueAttribute
    /// Can be used when the display and the underlying string value (used for persistance purposes)
    /// differ.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class StringEnumDisplayTextAttribute : Attribute
    {

        public StringEnumDisplayTextAttribute( string value, bool addDate = false )
        {
            //if ( addDate )
            //{
            //    value = $"{value} (e.g. {DateTime.Now.ToString( value )})";
            //}

            Value = value;
        }

        public string Value { get; set; }
    }
}
