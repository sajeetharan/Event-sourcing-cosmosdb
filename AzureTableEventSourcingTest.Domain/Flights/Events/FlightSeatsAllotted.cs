using System;

namespace AzureTableEventSourcingTest.Domain.Flights.Events
{
	public class FlightSeatsAllotted : IEvent
	{
		public FlightSeatsAllotted(FlightId flightId, int numberOfSeats)
		{
			FlightId = flightId;
			NumberOfSeats = numberOfSeats;
		}

		public FlightId FlightId { get; }
		public int NumberOfSeats { get; }

		public override string ToString() => $"{NumberOfSeats} seats allotted on flight {FlightId}.";
	}
}
