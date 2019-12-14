using System;
using System.ComponentModel;

namespace AzureTableEventSourcingTest.Domain.Flights
{
    [TypeConverter(typeof(FlightIdTypeConverter))]
    public class FlightId: IEquatable<FlightId>
    {
        public static FlightId New() => new FlightId(Guid.NewGuid());

        public static bool TryParse(string value, out FlightId result)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = new FlightId(guid);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static FlightId Parse(string value)
        {
            return TryParse(value, out var result)
                ? result
                : throw new FormatException($"'{value}' is not a valid {nameof(FlightId)} value.");
        }

        private readonly Guid value;

        private FlightId(Guid value)
        {
            this.value = value;
        }

        public override string ToString() => value.ToString();
        public override int GetHashCode() => value.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as FlightId);
        public bool Equals(FlightId other) => ReferenceEquals(other, this) || value == other?.value;
    }

    public class FlightIdTypeConverter: StringTypeConverter<FlightId>
    {
        public FlightIdTypeConverter(): base(FlightId.Parse)
        {
        }
    }
}
