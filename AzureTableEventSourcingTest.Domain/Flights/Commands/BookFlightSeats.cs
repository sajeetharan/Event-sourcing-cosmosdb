using System;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Domain.Flights.Commands
{
	public class BookFlightSeats : ICommandHandler<BookFlightSeats.Command, BookFlightSeats.Result>
	{
		public class Command
		{
			public Command(FlightId flightId, int numberOfSeats, string reservationId)
			{
				FlightId = flightId;
				NumberOfSeats = numberOfSeats;
				ReservationId = reservationId;
			}

			public FlightId FlightId { get; }
			public int NumberOfSeats { get; }
			public string ReservationId { get; }
		}

		public abstract class Result
		{
			public class Success : Result
			{
			}

			public class NotFound : Result
			{
			}

			public class Conflict : Result
			{
				public Conflict(string message)
				{
					Message = message ?? throw new ArgumentNullException(nameof(message));
				}

				public string Message { get; }
			}

			public class ConcurrentWrite : Result
			{
			}
		}

		private readonly IEventStore<FlightId, Flight> store;

		public BookFlightSeats(IEventStore<FlightId, Flight> store)
		{
			this.store = store ?? throw new ArgumentNullException(nameof(store));
		}

		public async Task<Result> ExecuteAsync(Command command)
		{
			try
			{
				await store.Update(command.FlightId, Flight.LoadFrom,
					f => f.BookSeats(command.NumberOfSeats, command.ReservationId));
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
