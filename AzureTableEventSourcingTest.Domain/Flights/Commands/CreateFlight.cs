using NodaTime;
using System;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Domain.Flights.Commands
{
	public class CreateFlight: ICommandHandler<CreateFlight.Command, CreateFlight.Result>
	{
		public class Command
		{
			public Command(IataAirportCode fromAirport, IataAirportCode toAirport, LocalTime departureTime, LocalTime arrivalTime)
			{
				FromAirport = fromAirport ?? throw new ArgumentNullException(nameof(fromAirport));
				ToAirport = toAirport ?? throw new ArgumentNullException(nameof(toAirport));
				DepartureTime = departureTime;
				ArrivalTime = arrivalTime;
			}

			public IataAirportCode FromAirport { get; }
			public IataAirportCode ToAirport { get; }
			public LocalTime DepartureTime { get; }
			public LocalTime ArrivalTime { get; }
		}

		public abstract class Result
		{
			public class Success : Result
			{
				public Success(FlightId flightId)
				{
					FlightId = flightId;
				}

				public FlightId FlightId { get; }
			}
		}

		private readonly IEventStore<FlightId, Flight> store;

		public CreateFlight(IEventStore<FlightId, Flight> store)
		{
			this.store = store ?? throw new ArgumentNullException(nameof(store));
		}
		
		public async Task<Result> ExecuteAsync(Command command)
		{
			var flightId = FlightId.New();
			var events = Flight.Create(flightId, command.FromAirport, command.ToAirport, command.DepartureTime, command.ArrivalTime);
			await store.CreateStreamAsync(flightId, events);
			return new Result.Success(flightId);
		}
	}
}
