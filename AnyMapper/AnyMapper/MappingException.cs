using System;
using System.Collections.Generic;
using System.Text;
using TypeSupport;

namespace AnyMapper
{
    /// <summary>
    /// A mapping exception
    /// </summary>
    public class MappingException : Exception
    {
        /// <summary>
        /// The source property name
        /// </summary>
        public Field SourceField { get; }
        /// <summary>
        /// The source property type
        /// </summary>
        public Field DestinationField { get; }

        public MappingException() : base()
        {

        }

        public MappingException(Field sourceField, Field destinationField)
            : this(sourceField, destinationField, $"Cannot map {FormatField(sourceField)} => {FormatField(destinationField)}")
        {
        }

        public MappingException(Field sourceField, Field destinationField, string message) : base(message)
        {
            SourceField = sourceField;
            DestinationField = destinationField;
        }

        public MappingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Format a field for error
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string FormatField(Field field)
        {
            return $"{field.DeclaringType.Type.Name}{field.Name}[{field.Type.Type.Name}]";
        }
    }
}
