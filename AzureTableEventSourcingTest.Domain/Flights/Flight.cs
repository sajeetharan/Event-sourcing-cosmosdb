using AzureTableEventSourcingTest.Domain.Flights.Events;
using NodaTime;
using System;
using System.Collections.Generic;

namespace AzureTableEventSourcingTest.Domain.Flights
{
	public class Flight: IAggregateRoot<FlightId>
	{
		public static IEnumerable<IEvent> Create(FlightId flightId, IataAirportCode fromAirport, IataAirportCode toAirport, LocalTime departureTime, LocalTime arrivalTime)
		{
			yield return new FlightCreated(flightId, fromAirport, toAirport, departureTime, arrivalTime);
		}

		public static Flight LoadFrom(IEnumerable<IEvent> events)
		{
			var flight = new Flight();
			foreach (var e in events)
			{
				flight.Apply(e);
			}
			return flight;
		}

		private FlightId _id;
		private int _numberOfSeats;
		private int _numberOfBookedSeats;

		private int NumberOfAvailableSeats => _numberOfSeats - _numberOfBookedSeats;

		private void Apply(IEvent @event)
		{
			switch (@event)
			{
				case FlightCreated flightCreated:
					Apply(flightCreated);
					break;
					
				case FlightSeatsAllotted flightAllotted:
					Apply(flightAllotted);
					break;

				case SeatsBooked seatsBooked:
					Apply(seatsBooked);
					break;
			}
		}

		private void Apply(FlightCreated @event)
		{
			_id = @event.FlightId;
		}
		
		private void Apply(FlightSeatsAllotted @event)
		{
			_numberOfSeats = @event.NumberOfSeats;
		}

		private void Apply(SeatsBooked @event)
		{
			_numberOfBookedSeats += @event.NumberOfSeats;
		}
		
		public bool CanAllotSeats(int numberOfSeats)
			=> _numberOfBookedSeats <= numberOfSeats;
		
		public IEnumerable<IEvent> AllotSeats(int numberOfSeats)
		{
			if (!CanAllotSeats(numberOfSeats))
			{
				throw new InvalidOperationException($"Already too many booked seats.");
			}

			yield return new FlightSeatsAllotted(_id, numberOfSeats);
		}

		public bool CanBookSeats(int numberOfSeats)
			=> NumberOfAvailableSeats >= numberOfSeats;

		public IEnumerable<IEvent> BookSeats(int numberOfSeats, string reservationId)
		{
			if (!CanBookSeats(numberOfSeats)) throw new InvalidOperationException($"Not enough available seats.");

			yield return new SeatsBooked(_id, numberOfSeats, reservationId);
		}
	}
}
