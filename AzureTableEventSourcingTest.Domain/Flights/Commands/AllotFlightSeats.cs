using System;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Domain.Flights.Commands
{
	public class AllotFlightSeats: ICommandHandler<AllotFlightSeats.Command, AllotFlightSeats.Result>
	{
		public class Command
		{
			public Command(FlightId flightId, int numberOfSeats)
			{
				FlightId = flightId;
				NumberOfSeats = numberOfSeats;
			}

			public FlightId FlightId { get; }
			public int NumberOfSeats { get; }
		}

		public abstract class Result
		{
			public class Success : Result
			{
			}

			public class NotFound: Result
			{
			}

			public class Conflict: Result
			{
				public Conflict(string message)
				{
					Message = message ?? throw new ArgumentNullException(nameof(message));
				}

				public string Message { get; }
			}

			public class ConcurrentWrite: Result
			{
			}
		}

		private readonly IEventStore<FlightId, Flight> store;

		public AllotFlightSeats(IEventStore<FlightId, Flight> store)
		{
			this.store = store ?? throw new ArgumentNullException(nameof(store));
		}

		public async Task<Result> ExecuteAsync(Command command)
		{
			try
			{
				await store.Update(command.FlightId, Flight.LoadFrom, f => f.AllotSeats(command.NumberOfSeats));
				return new Result.Success();
			}
			catch (StreamNotFoundException)
			{
				return new Result.NotFound();
			}
			catch (InvalidOperationException e)
			{
				return new Result.Conflict(e.Message);
			}
			catch (ConcurrencyException)
			{
				return new Result.ConcurrentWrite();
			}
		}
	}
}
