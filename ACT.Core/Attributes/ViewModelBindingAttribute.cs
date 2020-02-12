using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ACT.Core.Attributes
{
    public enum BindingMode
    {

        /// <summary>
        /// Implies that the Binding Source can populate the Model.
        /// </summary>
        OneWay = 1,

        /// <summary>
        /// Implies that the Binding Source can populate the Model and vica-versa.
        /// </summary>
        TwoWay = 2,

        WhenPopulated = 4
    }

    /// <summary>
    /// This attribute is used to assist the ViewModelHelper in identifying Properties that must be copied from
    /// and to Persited objects.  It is also used by the ModelForm and ModelElement html helpers to determin
    /// which keys must be present in a form or html element.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true )]
    public class ViewModelBindingAttribute : Attribute
    {

        public ViewModelBindingAttribute() : this( BindingMode.TwoWay, null ) { }

        public ViewModelBindingAttribute( object defaultValue ) : this( BindingMode.TwoWay, defaultValue ) { }

        public ViewModelBindingAttribute( BindingMode bindingMode )
            : this( bindingMode, null )
        {
        }

        public ViewModelBindingAttribute( BindingMode bindingMode, object defaultValue )
        {
            this.BindingMode = bindingMode;
            this.HasBindingType = false;
            this.DefaultValue = defaultValue;
        }



        public ViewModelBindingAttribute( Type bindingType ) : this( bindingType, null ) { }

        public ViewModelBindingAttribute( Type bindingType, object defaultValue )
        {
            this.BindingMode = Attributes.BindingMode.TwoWay;
            this.BindingType = bindingType;
            this.HasBindingType = true;
            this.DefaultValue = defaultValue;
        }




        public BindingMode BindingMode { get; set; }

        public bool HasBindingType { get; set; }

        public Type BindingType { get; set; }

        public PropertyInfo BindingProperty { get; set; }

        public object DefaultValue { get; set; }

        public string EncryptionKey { get; set; }


    }
}
