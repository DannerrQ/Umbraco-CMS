﻿using System;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Web.PropertyEditors
{
    /// <summary>
    /// Custom value editor which ensures that the value stored is just plain text and that
    /// no magic json formatting occurs when translating it to and from the database values
    /// </summary>
    public class TextOnlyValueEditor : ValueEditor
    {
        public TextOnlyValueEditor(ValueEditorAttribute attribute)
            : base(attribute)
        { }

        /// <summary>
        /// A method used to format the database value to a value that can be used by the editor
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyType"></param>
        /// <param name="dataTypeService"></param>
        /// <returns></returns>
        /// <remarks>
        /// The object returned will always be a string and if the database type is not a valid string type an exception is thrown
        /// </remarks>
        public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
        {
            if (property.GetValue() == null) return string.Empty;

            switch (ValueTypes.ToStorageType(ValueType))
            {
                case ValueStorageType.Ntext:
                case ValueStorageType.Nvarchar:
                    return property.GetValue().ToString();
                case ValueStorageType.Integer:
                case ValueStorageType.Decimal:
                case ValueStorageType.Date:
                default:
                    throw new InvalidOperationException("The " + typeof(TextOnlyValueEditor) + " can only be used with string based property editors");
            }
        }

    }
}
