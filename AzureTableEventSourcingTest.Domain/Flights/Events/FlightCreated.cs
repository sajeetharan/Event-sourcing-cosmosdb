using NodaTime;
using System;

namespace AzureTableEventSourcingTest.Domain.Flights.Events
{
	public class FlightCreated: IEvent
	{
		public FlightCreated(FlightId flightId, 
			IataAirportCode fromAirport, IataAirportCode toAirport, 
			LocalTime departureTime, LocalTime arrivalTime)
		{
			FlightId = flightId;
			FromAirport = fromAirport ?? throw new ArgumentNullException(nameof(fromAirport));
			ToAirport = toAirport ?? throw new ArgumentNullException(nameof(toAirport));
			DepartureTime = departureTime;
			ArrivalTime = arrivalTime;
		}

		public FlightId FlightId { get; }
		public IataAirportCode FromAirport { get; }
		public IataAirportCode ToAirport { get; }
		public LocalTime DepartureTime { get; }
		public LocalTime ArrivalTime { get; }

		public override string ToString() => $"Flight {FlightId} created ({FromAirport} -> {ToAirport}).";
	}
}
