using System;
using System.ComponentModel;
using System.Globalization;

namespace AzureTableEventSourcingTest.Domain
{
    public class StringTypeConverter<T>: TypeConverter
    {
        private readonly Func<string, T> fromString;
        private readonly Func<T, string> toString;

        public StringTypeConverter(Func<string, T> fromString)
            : this(fromString, v => v?.ToString())
        {
        }

        public StringTypeConverter(Func<string, T> fromString, Func<T, string> toString)
        {
            this.fromString = fromString ?? throw new ArgumentNullException(nameof(fromString));
            this.toString = toString ?? throw new ArgumentNullException(nameof(toString));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string)
                || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is T castedValue && destinationType == typeof(string))
            {
                return toString(castedValue);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return fromString(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
