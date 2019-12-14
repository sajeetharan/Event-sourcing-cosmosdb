using AzureTableEventSourcingTest.Domain;
using AzureTableEventSourcingTest.Domain.Flights;
using AzureTableEventSourcingTest.Domain.Flights.Commands;
using AzureTableEventSourcingTest.WebApi.Models.Flights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FlightsController : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> Create(
			[FromServices] ICommandHandler<CreateFlight.Command, CreateFlight.Result> createFlight,
			[FromBody] CreateFlightInputModel inputModel)
		{
			var result = await createFlight.ExecuteAsync(new CreateFlight.Command(
				inputModel.FromAirport,
				inputModel.ToAirport,
				inputModel.DepartureTime,
				inputModel.ArrivalTime));
			switch (result)
			{
				case CreateFlight.Result.Success success:
					return Ok(new { id = success.FlightId });

				default:
					throw new InvalidProgramException($"Unknown result: {result.GetType()}");
			}
		}
		
		[HttpPost("{id}/allotment")]
		public async Task<IActionResult> Allot(
			[FromServices] ICommandHandler<AllotFlightSeats.Command, AllotFlightSeats.Result> allotFlight,
			[FromRoute] FlightId id,
			[FromBody] AddAllotmentInputModel inputModel)
		{
			var result = await allotFlight.ExecuteAsync(new AllotFlightSeats.Command(
				id,
				inputModel.NumberOfSeats));
			switch (result)
			{
				case AllotFlightSeats.Result.Success _:
					return Accepted();

				case AllotFlightSeats.Result.NotFound _:
					return NotFound();

				case AllotFlightSeats.Result.Conflict conflict:
					return Conflict(new { conflict.Message });

				case AllotFlightSeats.Result.ConcurrentWrite _:
					return StatusCode(StatusCodes.Status503ServiceUnavailable);

				default:
					throw new InvalidProgramException($"Unknown result: {result.GetType()}");
			}
		}

		[HttpPost("{id}/booking")]
		public async Task<IActionResult> BookSeats(
			[FromServices] ICommandHandler<BookFlightSeats.Command, BookFlightSeats.Result> bookSeats,
			[FromRoute] FlightId id,
			[FromBody] BookSeatsInputModel inputModel)
		{
			var result = await bookSeats.ExecuteAsync(new BookFlightSeats.Command(
				id,
				inputModel.NumberOfSeats,
				inputModel.ReservationId));
			switch (result)
			{
				case BookFlightSeats.Result.Success _:
					return Accepted();

				case BookFlightSeats.Result.NotFound _:
					return NotFound();

				case BookFlightSeats.Result.Conflict conflict:
					return Conflict(new { conflict.Message });

				case BookFlightSeats.Result.ConcurrentWrite _:
					return StatusCode(StatusCodes.Status503ServiceUnavailable);

				default:
					throw new InvalidProgramException($"Unknown result: {result.GetType()}");
			}
		}
	}
}
