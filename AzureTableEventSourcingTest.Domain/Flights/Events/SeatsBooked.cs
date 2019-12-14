namespace AzureTableEventSourcingTest.Domain.Flights.Events
{
    public class SeatsBooked: IEvent
	{
		public SeatsBooked(FlightId flightId, int numberOfSeats, string reservationId)
		{
			FlightId = flightId;
			NumberOfSeats = numberOfSeats;
			ReservationId = reservationId;
		}

		public FlightId FlightId { get; }
		public int NumberOfSeats { get; }
		public string ReservationId { get; }

		public override string ToString() => $"{NumberOfSeats} seats booked on flight {FlightId} (reservation {ReservationId}).";
	}
}
