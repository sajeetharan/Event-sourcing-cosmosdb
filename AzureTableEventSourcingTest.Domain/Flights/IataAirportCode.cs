using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AzureTableEventSourcingTest.Domain.Flights
{
	[TypeConverter(typeof(IataAirportCodeTypeConverter))]
	public class IataAirportCode : IEquatable<IataAirportCode>
	{
		private static readonly Regex pattern = new Regex("^[a-z]{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static bool TryParse(string value, out IataAirportCode result)
		{
			if (!pattern.IsMatch(value))
			{
				result = default;
				return false;
			}

			result = new IataAirportCode(value.ToUpper());
			return true;
		}

		public static IataAirportCode Parse(string value)
		{
			return TryParse(value, out var result)
				? result
				: throw new FormatException($"'{value}' is not a valid {nameof(IataAirportCode)} value.");
		}

		private readonly string value;

		private IataAirportCode(string value)
		{
			this.value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public override string ToString() => value;
		public override int GetHashCode() => value.GetHashCode();
		public override bool Equals(object obj) => Equals(obj as IataAirportCode);
        public bool Equals(IataAirportCode other) => ReferenceEquals(other, this) || other?.value == value;
    }

	public class IataAirportCodeTypeConverter : StringTypeConverter<IataAirportCode>
	{
        public IataAirportCodeTypeConverter(): base(IataAirportCode.Parse)
        {
        }
	}
}
