using System;
using System.ComponentModel.DataAnnotations;

namespace AzureTableEventSourcingTest.WebApi.Models.Flights
{
	public class BookSeatsInputModel
	{
		[Required]
		public int NumberOfSeats { get; set; }

		[Required]
		public string ReservationId { get; set; }
	}
}
