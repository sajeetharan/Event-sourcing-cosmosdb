using AzureTableEventSourcingTest.Domain.Flights;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace AzureTableEventSourcingTest.WebApi.Models.Flights
{
	public class CreateFlightInputModel
	{
		[Required]
		public IataAirportCode FromAirport { get; set; }

		[Required]
		public IataAirportCode ToAirport { get; set; }

		[Required]
		public LocalTime DepartureTime { get; set; }

		[Required]
		public LocalTime ArrivalTime { get; set; }
	}
}
