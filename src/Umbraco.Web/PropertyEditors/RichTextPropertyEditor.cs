﻿using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Macros;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Web.PropertyEditors
{
    [ValueEditor(Constants.PropertyEditors.Aliases.TinyMce, "Rich Text Editor", "rte", ValueType = ValueTypes.Text,  HideLabel = false, Group="Rich Content", Icon="icon-browser-window")]
    public class RichTextPropertyEditor : PropertyEditor
    {
        /// <summary>
        /// The constructor will setup the property editor based on the attribute if one is found
        /// </summary>
        public RichTextPropertyEditor(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Create a custom value editor
        /// </summary>
        /// <returns></returns>
        protected override IPropertyValueEditor CreateValueEditor() => new RichTextPropertyValueEditor(Attribute);

        protected override ConfigurationEditor CreateConfigurationEditor() => new RichTextConfigurationEditor();


        /// <summary>
        /// A custom value editor to ensure that macro syntax is parsed when being persisted and formatted correctly for display in the editor
        /// </summary>
        internal class RichTextPropertyValueEditor : ValueEditor
        {
            public RichTextPropertyValueEditor(ValueEditorAttribute attribute)
                : base(attribute)
            { }

            /// <inheritdoc />
            public override object Configuration
            {
                get => base.Configuration;
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    if (!(value is RichTextConfiguration configuration))
                        throw new ArgumentException($"Expected a {typeof(RichTextConfiguration).Name} instance, but got {value.GetType().Name}.", nameof(value));
                    HideLabel = configuration.HideLabel;
                    base.Configuration = value;
                }

            }

            /// <summary>
            /// Format the data for the editor
            /// </summary>
            /// <param name="property"></param>
            /// <param name="propertyType"></param>
            /// <param name="dataTypeService"></param>
            /// <returns></returns>
            public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
            {
                if (property.GetValue() == null)
                    return null;

                var parsed = MacroTagParser.FormatRichTextPersistedDataForEditor(property.GetValue().ToString(), new Dictionary<string, string>());
                return parsed;
            }

            /// <summary>
            /// Format the data for persistence
            /// </summary>
            /// <param name="editorValue"></param>
            /// <param name="currentValue"></param>
            /// <returns></returns>
            public override object ConvertEditorToDb(Core.Models.Editors.ContentPropertyData editorValue, object currentValue)
            {
                if (editorValue.Value == null)
                    return null;

                var parsed = MacroTagParser.FormatRichTextContentForPersistence(editorValue.Value.ToString());
                return parsed;
            }
        }
    }


}
