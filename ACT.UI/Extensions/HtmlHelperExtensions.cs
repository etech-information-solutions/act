using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.IO;
using ACT.Core.Interfaces;
using ACT.Core.Enums;
using ACT.Core.Attributes;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        private static readonly string PLACEHOLDER = "__PlAcEhOlDeR__";

        #region ..FieldFor

        public static MvcHtmlString EditorFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.RenderField( expression, () =>
            {
                return html.EditorFor( expression );
            } );
        }

        public static MvcHtmlString EditorFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string templateName )
        {
            return html.RenderField( expression, () =>
            {
                return html.EditorFor( expression, templateName );
            } );
        }

        public static MvcHtmlString EditorFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string templateName, object additionalViewData )
        {
            return html.RenderField( expression, () =>
            {
                return html.EditorFor( expression, templateName, additionalViewData );
            } );
        }

        public static MvcHtmlString TextBoxFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.TextBoxFieldFor( expression, null );
        }

        public static MvcHtmlString TextBoxFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.TextBoxFor( expression, htmlAttributes );
            } );
        }



        public static MvcHtmlString TextAreaFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.TextAreaFieldFor( expression, 5, 20, null );
        }

        public static MvcHtmlString TextAreaFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, int rows, int columns, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.TextAreaFor( expression, rows, columns, htmlAttributes );
            } );
        }

        public static MvcHtmlString DecimalFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, int decimals )
        {
            return html.DecimalFieldFor( expression, decimals, null );
        }


        public static MvcHtmlString DecimalFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, int decimals, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.DecimalFor( expression, decimals, htmlAttributes );
            } );
        }

        public static MvcHtmlString DisplayTextFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.RenderField( expression, () =>
            {
                return html.DisplayTextFor( expression, true );
            }, false, false );
        }

        public static MvcHtmlString MailLinkFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression, Expression<Func<TModel, string>> displayExpression )
        {

            return html.RenderField( emailExpression, () =>
            {
                return html.MailLinkFor( emailExpression, displayExpression );
            }, false, false );
        }

        public static MvcHtmlString MailLinkFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression )
        {
            return html.RenderField( emailExpression, () =>
            {
                return html.MailLinkFor( emailExpression );
            }, false, false );
        }

        public static MvcHtmlString LinkFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression, Expression<Func<TModel, string>> displayExpression )
        {

            return html.RenderField( emailExpression, () =>
            {
                return html.LinkFor( emailExpression, displayExpression );
            }, false, false );
        }

        public static MvcHtmlString LinkFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression )
        {
            return html.RenderField( emailExpression, () =>
            {
                return html.LinkFor( emailExpression );
            }, false, false );
        }


        public static MvcHtmlString PasswordFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.PasswordFieldFor( expression, null );
        }

        public static MvcHtmlString PasswordFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.PasswordFor( expression, htmlAttributes );
            } );
        }


        public static MvcHtmlString EnumDropDownFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
            where TProperty : struct
        {

            return html.EnumDropDownFieldFor( expression, null );
        }


        public static MvcHtmlString EnumDropDownFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
            where TProperty : struct
        {

            return html.EnumDropDownFieldFor( expression, false, null, htmlAttributes );
        }

        public static MvcHtmlString EnumDropDownFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical, TProperty? flaggedEnumfilter, object htmlAttributes )
            where TProperty : struct
        {

            return html.RenderField( expression, () =>
            {
                return html.EnumDropDownFor( expression, orderAlphabetical, false, flaggedEnumfilter, htmlAttributes );
            } );
        }

        public static MvcHtmlString EnumDropDownFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Func<TProperty, string> displayTextFunction, bool orderAlphabetical, TProperty? flaggedEnumfilter, object htmlAttributes )
            where TProperty : struct
        {

            return html.RenderField( expression, () =>
            {
                return html.EnumDropDownFor( expression, displayTextFunction, orderAlphabetical, false, flaggedEnumfilter, htmlAttributes );
            } );
        }

        public static MvcHtmlString EnumRadiosFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
            where TProperty : struct
        {

            return html.EnumRadiosFieldFor( expression, false );

        }

        public static MvcHtmlString EnumRadiosFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical )
            where TProperty : struct
        {

            return html.RenderField( expression, () =>
            {
                return html.EnumRadiosFor( expression, orderAlphabetical );
            } );
        }

        public static MvcHtmlString EnumDisplayFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
            where TProperty : struct
        {

            return html.RenderField( expression, () =>
            {
                return html.EnumDisplayFor( expression );
            } );

        }


        public static MvcHtmlString DatePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression )
        {
            return html.DatePickerFieldFor( expression, null );
        }

        public static MvcHtmlString DatePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression, object htmlAttributes )
        {

            return html.RenderField( expression, () =>
            {
                return html.DatePickerFor( expression, htmlAttributes );
            } );

        }


        public static MvcHtmlString DatePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression )
        {
            return html.DatePickerFieldFor( expression, false, null );
        }

        public static MvcHtmlString DatePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression, bool nullable, object htmlAttributes )
        {

            return html.RenderField( expression, () =>
            {
                return html.DatePickerFor( expression, nullable, htmlAttributes );
            } );

        }

        public static MvcHtmlString DateTimePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression )
        {
            return html.DateTimePickerFieldFor( expression, null );
        }


        public static MvcHtmlString DateTimePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression, object htmlAttributes )
        {

            return html.RenderField( expression, () =>
            {
                return html.DateTimePickerFor( expression, htmlAttributes );
            } );

        }

        public static MvcHtmlString DateTimePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression )
        {
            return html.DateTimePickerFieldFor( expression, false, null );
        }


        public static MvcHtmlString DateTimePickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression, bool nullable, object htmlAttributes )
        {

            return html.RenderField( expression, () =>
            {
                return html.DateTimePickerFor( expression, nullable, htmlAttributes );
            } );

        }

        public static MvcHtmlString ColorPickerFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> expression )
        {

            return html.RenderField( expression, () =>
            {
                return html.ColorPickerFor( expression/*, true*/);
            } );

        }

        public static MvcHtmlString FileUploadFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.FileUploadFieldFor( expression, null );
        }

        public static MvcHtmlString FileUploadFieldFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.FileUploadFor( expression, htmlAttributes );
            } );
        }


        public static MvcHtmlString DropDownListFieldFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional )
        {
            return html.DropDownListFieldFor( expression, listExpression, valueExpression, textExpression, optional, "Select..." );
        }

        public static MvcHtmlString DropDownListFieldFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText )
        {
            return html.DropDownListFieldFor( expression, listExpression, valueExpression, textExpression, optional, defaultText, null );
        }

        public static MvcHtmlString DropDownListFieldFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, object htmlAttributes )
        {
            return html.DropDownListFieldFor( expression, listExpression, valueExpression, textExpression, optional, "Select...", htmlAttributes );
        }

        public static MvcHtmlString DropDownListFieldFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                return html.DropDownListFor( expression, listExpression, valueExpression, textExpression, optional, defaultText, htmlAttributes );
            } );
        }

        public static MvcHtmlString DropDownListFieldFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText, object htmlAttributes, Func<TListItem, object> optionHtmlAttributesFunc )
        {
            return html.RenderField( expression, () =>
            {
                return html.DropDownListFor( expression, listExpression, valueExpression, textExpression, optional, defaultText, htmlAttributes, optionHtmlAttributesFunc );
            } );
        }


        public static MvcHtmlString CheckBoxFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression )
        {
            return html.CheckBoxFieldFor( expression, null );
        }


        public static MvcHtmlString CheckBoxFieldFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, object htmlAttributes )
        {
            return html.RenderField( expression, () =>
            {
                //return html.CheckBoxFor(expression, htmlAttributes);
                //NB: Default Checkboxfor creates a hidden which causes problems
                return html.SoloCheckBoxFor( expression, htmlAttributes );

            } );
        }

        public static MvcHtmlString MultiSelectFieldFor<TModel, TItem, TItemKey>( this HtmlHelper<TModel> html,
                                                                                    Expression<Func<TModel, IEnumerable<TItemKey>>> linkedItemsExpression,
                                                                                    Expression<Func<TModel, IEnumerable<TItem>>> availableItemsExpression,
                                                                                    Expression<Func<TItem, string>> itemTextExpression )
            where TItem : IIdentifyable<TItemKey>
        {

            return html.RenderField( linkedItemsExpression, () =>
            {
                return html.MultiSelectFor( linkedItemsExpression, availableItemsExpression, itemTextExpression );
            } );

        }

        #endregion


        #region ColorPicker


        public static MvcHtmlString ColorPickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> expression/*, bool orderAlphabetical*/)
        {

            string name = expression.GetMemberInfo().Name;
            string mvcString = html.NullableHiddenFor( expression, null ).ToString();
            string value = "";
            try
            {
                value = expression.Compile().Invoke( html.ViewData.Model );
            }
            catch { }
            string colorPicker = string.Format( "<div class=\"color-input\" name=\"{0}-color-picker\" inputname= \"{0}\" selectedcolor=\"{1}\"></div>", name, value );
            return MvcHtmlString.Create( mvcString + colorPicker );

        }

        #endregion


        #region Enums

        public static MvcHtmlString EnumDropDownFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical )
            where TProperty : struct
        {

            return htmlHelper.EnumDropDownFor( expression, orderAlphabetical, false, null, null );
        }


        public static MvcHtmlString EnumDropDownFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical, object htmlAttributes )
            where TProperty : struct
        {

            return htmlHelper.EnumDropDownFor( expression, orderAlphabetical, false, null, htmlAttributes );
        }


        public static MvcHtmlString EnumDropDownFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical, bool optional, object htmlAttributes )
            where TProperty : struct
        {

            return htmlHelper.EnumDropDownFor( expression, orderAlphabetical, optional, null, htmlAttributes );
        }


        public static MvcHtmlString EnumDropDownFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper,
                                                                        Expression<Func<TModel, TProperty>> expression,
                                                                        bool orderAlphabetical,
                                                                        bool optional,
                                                                        TProperty? flaggedEnumFilter,
                                                                        object htmlAttributes )
                                                                        where TProperty : struct
        {

            return htmlHelper.EnumDropDownFor( expression, x => ( x as Enum ).GetDisplayText(), orderAlphabetical, optional, flaggedEnumFilter, htmlAttributes );

        }

        /// <summary>
        /// Generates a Drop Down Box.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="orderAlphabetical"></param>
        /// <param name="flaggedEnumFilter">Optional filter that can be used to constrain available options, provided that TOption is a Flagged Enumerator (1, 2, 4, 8, 16 etc)</param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper,
                                                                        Expression<Func<TModel, TProperty>> expression,
                                                                        Func<TProperty, string> displayTextFunction,
                                                                        bool orderAlphabetical,
                                                                        bool optional,
                                                                        TProperty? flaggedEnumFilter,
                                                                        object htmlAttributes )
                                                                        where TProperty : struct
        {


            string s = htmlHelper.DropDownListFor( expression, new SelectList( new List<object>() ), htmlAttributes ).ToString();

            //When the ViewModel is a property of another ViewModel, I get an issue where the "selected" attribute is not set correctly.
            //So we just use the existing htmlHelper.DropDownListFor function to get the correct id and name values

            //s = s.Substring(0, s.IndexOf(">") + 1);
            s = s.Substring( 0, s.ToLower().IndexOf( "</select>" ) );

            if ( optional )
            {
                s += string.Format( "<option value=\"\">{0}</option>", "Select..." );
            }

            TProperty selectedValue = ACT.Core.Helpers.EnumHelper.GetDefault<TProperty>();
            try
            {
                selectedValue = expression.Compile().Invoke( htmlHelper.ViewData.Model );
            }
            catch { }


            IEnumerable<TProperty> options;
            if ( orderAlphabetical )
            {
                options = ACT.Core.Helpers.EnumHelper.GetOptionsSortedAlphabetical<TProperty>();
            }
            else
            {
                //options = ACT.Core.Helpers.EnumHelper.GetOptions<TProperty>();
                options = ACT.Core.Helpers.EnumHelper.GetOptionsSortedNumeric<TProperty>();
            }

            string invalidOption = "";
            string validOptions = "";

            foreach ( TProperty enumOption in options )
            {

                if ( ( enumOption as Enum ).UiIgnore() )
                {
                    continue;
                }

                if ( flaggedEnumFilter.HasValue )
                {
                    if ( !( enumOption ).MatchesFilter( flaggedEnumFilter.Value ) )
                    {
                        continue;
                    }
                }

                string value = htmlHelper.Encode( ( enumOption as Enum ).GetIntValue().ToString() );
                string stringValue = htmlHelper.Encode( ( enumOption as Enum ).GetStringValue() );
                bool invalid = ( enumOption as Enum ).IsInvalid();

                //string text = htmlHelper.Encode((enumOption as Enum).GetDisplayText());
                string text = htmlHelper.Encode( displayTextFunction( enumOption ) );
                string selectedHtml = "";
                if ( object.Equals( enumOption, selectedValue ) )
                {
                    selectedHtml = "selected='selected'";
                }

                if ( invalid )
                {
                    value = "";
                    text = "Select...";
                    invalidOption += string.Format( "<option value=\"{0}\" {1} string-value=\"{3}\">{2}</option>", value, selectedHtml, text, stringValue );
                }
                else
                {
                    validOptions += string.Format( "<option value=\"{0}\" {1} string-value=\"{3}\">{2}</option>", value, selectedHtml, text, stringValue );
                }


            }

            s += invalidOption;
            s += validOptions;

            s += "</select>";
            return MvcHtmlString.Create( s );

        }



        public static MvcHtmlString EnumRadiosFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool orderAlphabetical )
            where TProperty : struct
        {

            string textBoxHtml = htmlHelper.TextBoxFor( expression ).ToString();


            MemberInfo memberInfo = expression.GetMemberInfo();

            string name = htmlHelper.GetAttributeValue( textBoxHtml, "name" );
            if ( string.IsNullOrEmpty( name ) )
            {
                name = memberInfo.Name;
            }

            string baseId = htmlHelper.GetAttributeValue( textBoxHtml, "id" );
            if ( string.IsNullOrEmpty( name ) )
            {
                baseId = memberInfo.Name;
            }

            TProperty selectedValue = ACT.Core.Helpers.EnumHelper.GetDefault<TProperty>();
            try
            {
                selectedValue = expression.Compile().Invoke( htmlHelper.ViewData.Model );
            }
            catch { }

            IEnumerable<TProperty> options;
            if ( orderAlphabetical )
            {
                options = ACT.Core.Helpers.EnumHelper.GetOptionsSortedAlphabetical<TProperty>();
            }
            else
            {
                options = ACT.Core.Helpers.EnumHelper.GetOptions<TProperty>();
            }

            string s = "<table class='tenum'><tr>";
            foreach ( TProperty enumOption in options )
            {

                string value = ( enumOption as Enum ).GetIntValue().ToString();
                bool invalid = ( enumOption as Enum ).IsInvalid();
                string style = "";
                if ( invalid )
                {
                    //value = "";
                    style = "style='display:none;'";
                    continue;
                }
                string text = ( enumOption as Enum ).GetDisplayText();
                string id = string.Format( "{0}-{1}", baseId, value );


                string checkedHtml = "";
                if ( object.Equals( enumOption, selectedValue ) )
                {
                    checkedHtml = "checked='checked'";
                }



                s += string.Format( "<td width='20px' {5}><input id='{0}' name='{1}' type='radio' value='{2}' {3} /></td><td {5}><label for='{0}' class='equal-width'>{4}</label></td>",
                                            id,                     //{0}
                                            name,                   //{1}
                                            value,                  //{2}
                                            checkedHtml,            //{3}
                                            text,                   //{4}
                                            style );                 //{5}

            }


            s += "</tr></table>";

            return MvcHtmlString.Create( s );

        }


        public static MvcHtmlString EnumDisplayFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
            where TProperty : struct
        {

            TProperty selectedValue = ACT.Core.Helpers.EnumHelper.GetDefault<TProperty>();
            try
            {
                selectedValue = expression.Compile().Invoke( html.ViewData.Model );
            }
            catch { }

            string s = selectedValue.ToString();
            Enum enm = ( selectedValue as Enum );
            if ( enm != null )
            {
                s = enm.GetDisplayText();
            }

            return MvcHtmlString.Create( s );
        }

        #endregion


        #region DatePicker

        public static MvcHtmlString DatePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression )
        {
            return html.DatePickerFor( expression, null );
        }

        public static MvcHtmlString DatePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression, object htmlAttributes )
        {

            string name = html.GetElementName( expression );

            string value = "";
            try
            {
                value = expression.Compile().Invoke( html.ViewData.Model ).ToString( "yyyy/MM/dd" ); //TODO: American format?
            }
            catch { }

            if ( value == "0001/01/01" )
            {
                value = "";
            }

            IDictionary<string, object> htmlAttributesDictionary = htmlAttributes.ToDictionary();

            htmlAttributesDictionary.AddOrAppend( "class", "date-picker", " " );
            htmlAttributesDictionary.AddOrAppend( "id", name, "-" );

            return html.TextBox( name, value, htmlAttributesDictionary );

        }

        public static MvcHtmlString DatePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression )
        {
            return html.DatePickerFor( expression, true, null );
        }

        public static MvcHtmlString DatePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression, bool nullable, object htmlAttributes )
        {

            //string name = expression.GetMemberInfo().Name;
            string name = html.GetElementName( expression );

            string value = "";
            try
            {
                value = expression.Compile().Invoke( html.ViewData.Model ).Value.ToString( "yyyy/MM/dd" ); //TODO: American format?
            }
            catch { }

            string clearButton = "";
            if ( nullable )
            {
                //clearButton = string.Format("<button id='clear-{0}' type='button' class='clear-date-picker'>Clear</button>", name);
                clearButton = string.Format( "<a id='clear-{0}' class='clear-date-picker button dark'>Clear</a>", name );
            }

            IDictionary<string, object> htmlAttributesDictionary = htmlAttributes.ToDictionary();

            htmlAttributesDictionary.AddOrAppend( "class", "date-picker", " " );
            htmlAttributesDictionary.AddOrAppend( "id", name, "-" );

            string textBox = html.TextBox( name, value, htmlAttributesDictionary ).ToString();

            return MvcHtmlString.Create( string.Format( textBox + clearButton ) );

        }

        #endregion


        #region DateTimePicker

        public static MvcHtmlString DateTimePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime>> expression, object htmlAttributes )
        {

            string name = html.GetElementName( expression );

            string value = "";
            try
            {
                value = expression.Compile().Invoke( html.ViewData.Model ).ToDefaultString();//TODO: American format?
            }
            catch { }


            IDictionary<string, object> htmlAttributesDictionary = htmlAttributes.ToDictionary();

            htmlAttributesDictionary.AddOrAppend( "class", "date-time-picker", " " );
            htmlAttributesDictionary.AddOrAppend( "id", name, "-" );

            return html.TextBox( name, value, htmlAttributesDictionary );

        }


        public static MvcHtmlString DateTimePickerFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression, bool nullable, object htmlAttributes )
        {


            string name = html.GetElementName( expression );

            string value = "";
            try
            {
                value = expression.Compile().Invoke( html.ViewData.Model ).Value.ToDefaultString();
            }
            catch { }

            string clearButton = "";
            if ( nullable )
            {
                //clearButton = string.Format("<button id='clear-{0}' type='button' class='clear-date-picker'>Clear</button>", name);
                clearButton = string.Format( "<a id='clear-{0}' class='clear-date-picker button dark'>Clear</a>", name );
            }

            IDictionary<string, object> htmlAttributesDictionary = htmlAttributes.ToDictionary();

            htmlAttributesDictionary.AddOrAppend( "class", "date-time-picker", " " );
            htmlAttributesDictionary.AddOrAppend( "id", name, "-" );


            string textBox = html.TextBox( name, value, htmlAttributesDictionary ).ToString();

            return MvcHtmlString.Create( textBox + clearButton );

        }


        #endregion


        #region FieldLayout

        public static MvcHtmlString RenderField<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Func<object> renderFunction )
        {
            return html.RenderField( expression, renderFunction, true, true );
        }

        public static MvcHtmlString RenderField<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Func<object> renderFunction, bool flagRequired, bool includeValidation )
        {

            return MvcHtmlString.Create( html.GetFieldLayout( expression, flagRequired, includeValidation ).Replace( PLACEHOLDER, renderFunction().ToString() ) );


        }

        public static string GetFieldLayout<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool flagRequired, bool includeValidation )
        {

            string s = html.GetOpenFieldHtml( expression, flagRequired, includeValidation );

            s += PLACEHOLDER;

            s += html.GetCloseFieldHtml( expression, includeValidation );

            return s;

        }

        public static string GetOpenFieldHtml<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool flagRequired, bool includeValidation )
        {
            string name = html.GetElementName( expression );

            MemberInfo member = expression.GetMemberInfo( html.ViewData.Model, FindMode.FindClass );

            bool required = member.HasAttribute<RequiredAttribute>();
            bool error = ( html.ViewData.ModelState.Keys.Contains( name )
                        && html.ViewData.ModelState[ name ].Errors.NullableAny() );


            string classNames = string.Format( "input-field field-for-{0}", name );
            if ( flagRequired && required )
            {
                classNames += " required";
            }

            if ( includeValidation && error )
            {
                classNames += " has-error";
            }

            string s = string.Format( "<div class='{0}'>", classNames );

            s += html.LabelFor( expression, flagRequired );
            return s;
        }

        public static string GetCloseFieldHtml<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool includeValidation )
        {
            string s = "";
            if ( includeValidation )
            {
                MvcHtmlString validationMessageHtml = html.ValidationMessageFor( expression );
                if ( validationMessageHtml != null )
                {
                    s += validationMessageHtml.ToString().CamelToSpaced();
                }
            }
            s += "</div>";

            return s;
        }

        #endregion


        #region LabelFor


        public static MvcHtmlString LabelFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool flagRequired )
        {
            MemberInfo member = expression.GetMemberInfo( html.ViewData.Model, FindMode.FindClass );

            string labelText;
            if ( member.HasAttribute<InstanceSpecificDisplayName>() )
            {
                labelText = member.GetAttribute<InstanceSpecificDisplayName>().GetDisplayName( html.ViewData.Model );
            }
            else
            {
                labelText = member.GetDisplayName();
            }
            return html.LabelFor( expression, labelText, flagRequired );
        }

        public static MvcHtmlString LabelFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, bool flagRequired )
        {

            MemberInfo member = expression.GetMemberInfo( html.ViewData.Model, FindMode.FindClass );

            if ( flagRequired && member.HasAttribute<RequiredAttribute>() )
            {
                labelText += "*";
            }

            return html.LabelFor( expression, labelText );
        }

        #endregion


        #region NullableHiddenFor

        //MVC renders no html element when the expression passed to HiddenFor evaulates as null.
        //This implies there there will be no clientside element of which the value can be set 
        //from Javascript

        public static MvcHtmlString NullableHiddenFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.NullableHiddenFor( expression, null );
        }

        public static MvcHtmlString NullableHiddenFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
        {
            return html.NullableHiddenFor( expression, htmlAttributes.ToDictionary() );
        }

        public static MvcHtmlString NullableHiddenFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes )
        {

            if ( htmlAttributes == null )
            {
                htmlAttributes = new Dictionary<string, object>();
            }

            htmlAttributes.AddOrAppend( "style", "display:none", ";" );

            return html.TextBoxFor( expression, htmlAttributes );

        }



        #endregion


        #region GetAttributeValue


        public static string GetAttributeValue<TModel>( this HtmlHelper<TModel> htmlHelper, string elementHtmlString, string attributeName )
        {

            try
            {


                string pattern = string.Format( "{0}=[',\"](?<value>.*?)[',\"]", attributeName );

                Regex regex = new Regex( pattern );

                foreach ( Match match in regex.Matches( elementHtmlString ) )
                {

                    return match.Groups[ "value" ].Value;
                }

                return null;

            }
            catch ( Exception )
            {
                //Not found
                return null;
            }


        }

        #endregion


        #region AbsoluteUrl

        public static MvcHtmlString AbsoluteUrl( this HtmlHelper html, string url )
        {

            if ( !string.IsNullOrEmpty( url ) )
            {

                if ( !( url.StartsWith( "http://" ) || url.StartsWith( "https://" ) ) )
                {
                    url = "http://" + url;
                }
            }

            return MvcHtmlString.Create( url );

        }


        #endregion


        #region Mailto

        public static MvcHtmlString MailTo( this HtmlHelper html, string email )
        {

            if ( !string.IsNullOrEmpty( email ) )
            {
                email = "mailto:" + email;
            }

            return MvcHtmlString.Create( email );

        }

        #endregion


        #region MailLinkFor


        public static MvcHtmlString MailLinkFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression )
        {
            return html.MailLinkFor( emailExpression, emailExpression );
        }

        public static MvcHtmlString MailLinkFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> emailExpression, Expression<Func<TModel, string>> displayExpression )
        {

            string email = emailExpression.Compile().Invoke( html.ViewData.Model );
            string display = displayExpression.Compile().Invoke( html.ViewData.Model );

            return MvcHtmlString.Create( string.Format( "<a href='{0}'>{1}</a>", html.MailTo( email ), display ) );

        }

        #endregion


        #region LinkFor


        public static MvcHtmlString LinkFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> urlExpression )
        {
            return html.LinkFor( urlExpression, urlExpression );
        }

        public static MvcHtmlString LinkFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, string>> urlExpression, Expression<Func<TModel, string>> displayExpression )
        {

            string url = urlExpression.Compile().Invoke( html.ViewData.Model );
            string display = displayExpression.Compile().Invoke( html.ViewData.Model );

            return MvcHtmlString.Create( string.Format( "<a href='{0}'>{1}</a>", url, display ) );

        }

        #endregion


        #region FileUploader

        public static MvcHtmlString FileUploadFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            return html.FileUploadFor( expression, null );
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes )
        {

            string s = html.TextBoxFor( expression, htmlAttributes )
                                    .ToString()
                                    .Replace( "type=\"text\"", "type=\"file\"" )
                                    .Replace( "type='text'", "type='file'" );


            return MvcHtmlString.Create( s );

        }


        #endregion


        #region Month Dropdown

        //public static MvcHtmlString MonthDropDownFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, int>> expression, object htmlAttributes )
        //{
        //    var months = System.Linq.Enumerable.Range( 1, 12 ).Select( i => new { I = i, M = DateTimeFormatInfo.CurrentInfo.GetMonthName( i ) } );

        //    return html.TextBox( name, value, htmlAttributesDictionary );

        //}

        #endregion


        #region DropDownFor IEnumerable

        public static MvcHtmlString DropDownListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional )
        {
            return html.DropDownListFor( expression, listExpression, valueExpression, textExpression, optional, "Select...", null );
        }

        public static MvcHtmlString DropDownListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText )
        {
            return html.DropDownListFor( expression, listExpression, valueExpression, textExpression, optional, defaultText, null );
        }

        public static MvcHtmlString DropDownListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, object htmlAttributes )
        {
            return html.DropDownListFor( expression, listExpression, valueExpression, textExpression, optional, "Select...", htmlAttributes );
        }

        public static MvcHtmlString DropDownListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText, object htmlAttributes )
        {

            //Use Mvc plumbing to generate correct id and name tags:
            IEnumerable<TListItem> items = listExpression.Compile().Invoke( html.ViewData.Model ) as IEnumerable<TListItem>;

            return html.DropDownListFor( expression, items.AsSelectList( valueExpression, textExpression, optional, defaultText ), htmlAttributes );

        }

        public static MvcHtmlString DropDownListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional, string defaultText, object htmlAttributes, Func<TListItem, object> optionAttributesFunc )
        {
            IEnumerable<TListItem> items = listExpression.Compile().Invoke( html.ViewData.Model ) as IEnumerable<TListItem>;

            string s = html.DropDownListFor( expression, new List<SelectListItem>(), htmlAttributes ).ToString().Replace( "</select>", "" );

            Func<TListItem, TSelected> valueFunc = valueExpression.Compile();
            Func<TListItem, string> textFunc = textExpression.Compile();

            TSelected selectedValue = expression.Compile().Invoke( html.ViewData.Model );


            if ( optional )
            {
                s += string.Format( "<option value=\"\">{0}</option>", defaultText );
            }

            foreach ( TListItem item in items )
            {
                string optionAttributesString = "";
                if ( optionAttributesFunc != null )
                {
                    optionAttributesString = optionAttributesFunc( item ).ToDictionary().ToHtmlAttributes();
                }

                string selectedAttribute = "";

                if ( valueFunc( item ).Equals( selectedValue ) )
                {
                    selectedAttribute = "selected=\"selected\"";
                }

                s += string.Format( "<option value=\"{0}\" {2} {3}>{1}</option>", valueFunc( item ), textFunc( item ), optionAttributesString, selectedAttribute );
            }

            s += "</select>";

            return MvcHtmlString.Create( s );
        }

        #endregion


        #region Js & Css


        public static MvcHtmlString Js( this HtmlHelper html, string path )
        {
            return html.Js( path, true );
        }


        public static MvcHtmlString Js( this HtmlHelper html, string path, bool forceNoCache )
        {

            string resolvedPath;
            try
            {
                resolvedPath = VirtualPathUtility.ToAbsolute( path );
            }
            catch
            {
                //assume absolute
                resolvedPath = path;
            }

            if ( forceNoCache )
            {
                DateTime date = DateTime.Now;
                Controller controller = ( html.ViewContext.Controller as Controller );
                if ( controller != null )
                {
                    string localPath = controller.Server.MapPath( path );
                    if ( File.Exists( localPath ) )
                    {
                        FileInfo f = new FileInfo( localPath );
                        date = f.LastWriteTime;
                    }
                }

                resolvedPath += "?" + date.ToString( "yyyyMMddHHmmssfff" );
            }

            string s = string.Format( "<script type=\"text/javascript\" language=\"javascript\" src='{0}'></script>", resolvedPath );

            return MvcHtmlString.Create( s );

        }


        public static MvcHtmlString Css( this HtmlHelper html, string path )
        {
            return html.Css( path, true );
        }


        public static MvcHtmlString Css( this HtmlHelper html, string path, bool forceNoCache )
        {

            string resolvedPath;
            try
            {
                resolvedPath = VirtualPathUtility.ToAbsolute( path );
            }
            catch
            {
                //assume absolute
                resolvedPath = path;
            }

            if ( forceNoCache )
            {
                DateTime date = DateTime.Now;
                Controller controller = ( html.ViewContext.Controller as Controller );
                if ( controller != null )
                {
                    string localPath = controller.Server.MapPath( path );
                    if ( File.Exists( localPath ) )
                    {
                        FileInfo f = new FileInfo( localPath );
                        date = f.LastWriteTime;
                    }
                }

                resolvedPath += "?" + date.ToString( "yyyyMMddHHmmssfff" );
            }

            string s = string.Format( "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", resolvedPath );

            return MvcHtmlString.Create( s );

        }


        #endregion


        #region Img

        public static MvcHtmlString Img( this HtmlHelper html, string path )
        {
            return html.Img( path, null );
        }

        public static MvcHtmlString Img( this HtmlHelper html, string path, object attributeValues )
        {


            string resolvedPath;
            try
            {
                resolvedPath = VirtualPathUtility.ToAbsolute( path );
            }
            catch
            {
                //assume absolute
                resolvedPath = path;
            }

            string s = string.Format( "<img src='{0}' {1} />", resolvedPath, attributeValues.ToDictionary().ToHtmlAttributes() );
            return MvcHtmlString.Create( s );

        }



        #endregion


        #region CheckBox

        public static MvcHtmlString SoloCheckBoxFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression )
        {
            return html.SoloCheckBoxFor( expression, null );
        }

        /// <summary>
        /// The default CheckBoxFor extension method provided by Microsoft adds a hidden input which can cause issues in Javascript.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString SoloCheckBoxFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, object htmlAttributes )
        {
            string id = html.GetElementId( expression );
            string name = html.GetElementName( expression );

            MemberInfo member = expression.GetMemberInfo();

            //MS CheckBoxFor actually inserts a checkbox and a hidden.  Both with the same name!
            bool isChecked = false;
            string checkedAttribute = "";
            try
            {
                isChecked = expression.Compile().Invoke( html.ViewData.Model );
            }
            catch { }

            if ( isChecked )
            {
                checkedAttribute = "checked='checked'";
            }

            IDictionary<string, object> htmlAttributesDict = htmlAttributes.ToDictionary();

            //foreach (KeyValuePair<string, object> attribute in htmlAttributesDict) {
            //    htmlAttributeString += string.Format("{0}='{1}'", attribute.Key, attribute.Value);
            //}

            string htmlAttributeString = htmlAttributesDict.ToHtmlAttributes();

            string checkBoxHtml = string.Format( "<input name='{0}' type='checkbox' {1} {2} id='{3}' />", name, checkedAttribute, htmlAttributeString, id );
            return MvcHtmlString.Create( checkBoxHtml );
        }

        #endregion


        #region LabelledCheckBoxFor

        public static MvcHtmlString LabelledCheckBoxFor<TModel>( this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, bool labelInFront, object htmlAttributes )
        {

            string name = html.GetElementName( expression );
            MemberInfo member = expression.GetMemberInfo();

            string labelHtml = html.LabelFor( expression, true ).ToString();


            //MS CheckBoxFor actually inserts a checkbox and a hidden.  Both with the same name!
            bool isChecked = false;
            string checkedAttribute = "";
            try
            {
                isChecked = expression.Compile().Invoke( html.ViewData.Model );
            }
            catch { }

            if ( isChecked )
            {
                checkedAttribute = "checked='checked'";
            }
            string htmlAttributeString = "";
            IDictionary<string, object> htmlAttributesDict = htmlAttributes.ToDictionary();
            foreach ( KeyValuePair<string, object> attribute in htmlAttributesDict )
            {
                htmlAttributeString += string.Format( "{0} = '{1}'", attribute.Key, attribute.Value );
            }

            string checkBoxHtml = string.Format( "<input name='{0}' type='checkbox' {1} {2}/>", name, checkedAttribute, htmlAttributeString );

            string fmt = "{0}&nbsp;&nbsp;{1}";
            string s;
            if ( labelInFront )
            {
                s = string.Format( fmt, labelHtml, checkBoxHtml );
            }
            else
            {
                s = string.Format( fmt, checkBoxHtml, labelHtml );
            }

            return MvcHtmlString.Create( s );

        }


        #endregion


        #region DecimalFor

        public static MvcHtmlString DecimalFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int decimals )
        {
            return html.DecimalFor( expression, decimals, null );
        }

        public static MvcHtmlString DecimalFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int decimals, object htmlAttributes )
        {


            string name = html.GetElementName( expression );


            double? dbl = null;
            try
            {
                dbl = Convert.ToDouble( expression.Compile().Invoke( html.ViewData.Model ) );
            }
            catch { }


            IDictionary<string, object> htmlAttributesDict = htmlAttributes.ToDictionary();
            if ( htmlAttributesDict.ContainsKey( "class" ) )
            {
                htmlAttributesDict[ "class" ] += " decimal-field";
            }
            else
            {
                htmlAttributesDict.Add( "class", "decimal-field" );
            }

            if ( dbl.HasValue )
            {
                return html.TextBox( name, dbl.Value.ToString( decimals ), htmlAttributesDict );
            }
            else
            {
                return html.TextBox( name, "", htmlAttributesDict );
            }

        }

        #endregion


        #region MultiSelectFor


        public static MvcHtmlString MultiSelectFor<TModel, TItem, TItemKey>( this HtmlHelper<TModel> html,
                                                                                    Expression<Func<TModel, IEnumerable<TItemKey>>> linkedItemsExpression,
                                                                                    Expression<Func<TModel, IEnumerable<TItem>>> availableItemsExpression,
                                                                                    Expression<Func<TItem, string>> itemTextExpression )
            where TItem : IIdentifyable<TItemKey>
        {


            string name = html.GetElementName( linkedItemsExpression );
            string handle = Guid.NewGuid().ToString();
            string delimitedValue = linkedItemsExpression.Compile().Invoke( html.ViewData.Model ).Delimit();

            //string s = string.Format("<div class='multi-select-container' id='{0}'>", handle);
            string s = string.Format( "<table class='multi-select-container-table'><tr><td class='multi-select-container' id='{0}'>", handle );

            //AutoSuggest wants this:
            s += html.TextBox( name + "-multi-select-ui", "", new { @class = "multi-select-ui" } );

            //s += "</div>";
            s += "</td></tr></table>";

            s += string.Format( "<select style='display:none;' id='{0}-options'>", handle );

            Func<TItem, string> textFunc = itemTextExpression.Compile();
            foreach ( TItem item in availableItemsExpression.Compile().Invoke( html.ViewData.Model ) )
            {
                s += string.Format( "<option value='{0}'>{1}</option>", item.ID, textFunc( item ) );
            }


            s += "</select>";

            s += html.Hidden( name, delimitedValue, new { id = string.Format( "{0}-values", handle ) } );

            return MvcHtmlString.Create( s );

        }


        #endregion


        #region DisplayTextFor

        public static MvcHtmlString DisplayTextFor<TModel, TResult>( this HtmlHelper<TModel> html, Expression<Func<TModel, TResult>> expression, bool enableEnumDisplayText )
        {

            if ( typeof( TResult ).IsEnum && enableEnumDisplayText )
            {
                try
                {
                    return MvcHtmlString.Create( ( expression.Compile().Invoke( html.ViewData.Model ) as Enum ).GetDisplayText() );
                }
                catch
                {
                }
            }

            if ( typeof( TResult ) == typeof( DateTime ) )
            {
                try
                {
                    DateTime dt = Convert.ToDateTime( expression.Compile().Invoke( html.ViewData.Model ) );
                    return MvcHtmlString.Create( dt.ToString( "MMM d, yyyy @HH:mm tt" ) );
                }
                catch
                {
                }
            }

            return html.DisplayTextFor( expression );
        }

        #endregion


        #region RadioButtonListFor

        public static MvcHtmlString RadioButtonListFor<TModel, TSelected, TListItem>( this HtmlHelper<TModel> html, Expression<Func<TModel, TSelected>> expression, Expression<Func<TModel, IEnumerable<TListItem>>> listExpression, Expression<Func<TListItem, TSelected>> valueExpression, Expression<Func<TListItem, string>> textExpression, bool optional = true, bool showhide = false, object htmlAttributes = null, string holdClass = "" )
        {
            int count = 0;
            string s = string.Empty;

            IEnumerable<TListItem> items = listExpression.Compile().Invoke( html.ViewData.Model ) as IEnumerable<TListItem>;

            Func<TListItem, TSelected> valueFunc = valueExpression.Compile();
            Func<TListItem, string> textFunc = textExpression.Compile();

            TSelected selectedValue = expression.Compile().Invoke( html.ViewData.Model );

            foreach ( TListItem item in items )
            {
                string show = string.Format( ".{0}", textFunc( item ).Replace( " ", "-" ).Replace( "(", "" ).Replace( ")", "" ).ToLower() );

                s += string.Format( "<div class=\"{0}\">", holdClass );

                s += html.RadioButtonFor( expression, valueFunc( item ), htmlAttributes ).ToString()
                    .Replace( "/>", "" )
                    .Replace( "id=\"", "id=\"" + count + "-" )
                    .Replace( "data-show=\"", string.Format( "data-show=\"{0}", show ) )
                    .Replace( "data-text=\"", string.Format( "data-text=\"{0}", textFunc( item ).ToLower().Replace( " ", "-" ).Replace( "(", "" ).Replace( ")", "" ) ) );

                string selectedAttribute = "";

                if ( valueFunc( item ).Equals( selectedValue ) || ( count == 0 ) )
                {
                    selectedAttribute = "checked=\"checked\"";
                }

                s += string.Format( "{0}{1}", selectedAttribute, " />" );

                s += html.LabelForModel( textFunc( item ), new { @for = string.Format( "{0}-{1}", count, expression.Body.ToString().Replace( "{", "" ).Replace( "}", "" ).Replace( "model.", "" ) ) } ).ToString();

                s += string.Format( "</div>" );

                count++;
            }

            return MvcHtmlString.Create( s );

        }


        #endregion


        #region AbsoluteApplicationUrl

        private static string _bestGuessUrl = null;
        private static Dictionary<string, string> _bestGuessUserUrl = new Dictionary<string, string>();


        /// <summary>
        /// Gets the application Url based on the current request.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString AbsoluteApplicationUrl( this HtmlHelper html )
        {

            //html.ViewContext.Controller.ControllerContext.HttpContext.Us

            Web.Routing.RouteData routeData = html.ViewContext.Controller.ControllerContext.RouteData;

            string controller = routeData.GetRequiredString( "controller" );
            string action = routeData.GetRequiredString( "action" );
            string controllerAction = string.Format( "/{0}/{1}", controller, action );

            HttpRequestBase req = html.ViewContext.RequestContext.HttpContext.Request;
            string appUrl = req.Url.AbsoluteUri;
            if ( appUrl.Contains( controllerAction ) )
            {
                appUrl = appUrl.Substring( 0, appUrl.IndexOf( controllerAction ) );
            }
            else if ( appUrl.EndsWith( "/" + controller + "/" ) )
            {
                appUrl.Substring( 0, appUrl.Length - ( controller.Length - 1 ) );
            }
            else if ( appUrl.EndsWith( "/" + controller ) )
            {
                appUrl.Substring( 0, appUrl.Length - ( controller.Length ) );
            }

            if ( string.IsNullOrEmpty( _bestGuessUrl ) || !appUrl.ToLower().Contains( "localhost" ) )
            {
                _bestGuessUrl = appUrl;
            }

            return MvcHtmlString.Create( appUrl );

        }

        /// <summary>
        /// Gets the application url based on incoming urls used.  If multiple urls are used
        /// over the life cycle of the application, then the most recent url is used, but a urls 
        /// that do not contain the phrase "localhost" will not take preference
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString BestGuessAbsoluteApplicationUrl( this HtmlHelper html )
        {

            MvcHtmlString appUrl = html.AbsoluteApplicationUrl();

            if ( string.IsNullOrEmpty( _bestGuessUrl ) || _bestGuessUrl.Contains( "localhost" ) )
            {
                return appUrl;
            }

            return MvcHtmlString.Create( _bestGuessUrl );
        }


        public static MvcHtmlString UpdateBestGuessApplicationUrlForCurrentUser( this HtmlHelper html )
        {

            if ( html.ViewContext.HttpContext.Request.IsAuthenticated )
            {

                string url = html.BestGuessAbsoluteApplicationUrl().ToString();

                string username = html.ViewContext.HttpContext.User.Identity.Name;
                if ( !string.IsNullOrEmpty( username ) && ( !_bestGuessUserUrl.ContainsKey( username ) || ( _bestGuessUserUrl[ username ].ToLower() != url.ToLower() ) ) )
                {
                    lock ( username )
                    {
                        try
                        {
                            _bestGuessUserUrl.AddOrOverwrite( username, url );
                        }
                        catch ( ArgumentException ) { } //Another thread for the same user?
                    }
                }
            }

            return MvcHtmlString.Empty;
        }


        /// <summary>
        /// Tries to determine the application url, but gives preference to the last url used
        /// by the specified userName.  This is in an attempt to cater for a scenario where some users
        /// access the Web Application on one URL and other users on another.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static MvcHtmlString BestGuessAbsoluteApplicationUrl( this HtmlHelper html, string username )
        {

            MvcHtmlString result;

            if ( string.IsNullOrEmpty( username ) )
            {
                result = html.BestGuessAbsoluteApplicationUrl();
                return result;
            }

            if ( _bestGuessUserUrl.ContainsKey( username ) )
            {
                return MvcHtmlString.Create( _bestGuessUserUrl[ username ] );
            }


            result = html.BestGuessAbsoluteApplicationUrl();
            return result;


        }



        #endregion


        #region Resolve

        public static string Resolve( this HtmlHelper html, string path )
        {
            try
            {
                return VirtualPathUtility.ToAbsolute( path );
            }
            catch ( Exception )
            {
                return "";
            }
        }

        #endregion


        #region GetElementId and GetElementName

        public static string GetElementId<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            string s = html.TextBoxFor( expression ).ToString();
            return html.GetAttributeValue( s, "id" ) ?? "no-id";
        }


        public static string GetElementName<TModel, TProperty>( this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression )
        {
            string s = html.TextBoxFor( expression ).ToString();
            return html.GetAttributeValue( s, "name" ) ?? "no-name";
        }

        #endregion

    }
}
