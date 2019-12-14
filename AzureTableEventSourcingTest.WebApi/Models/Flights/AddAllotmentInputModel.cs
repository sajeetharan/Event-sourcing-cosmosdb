using System.ComponentModel.DataAnnotations;

namespace AzureTableEventSourcingTest.WebApi.Models.Flights
{
	public class AddAllotmentInputModel
	{
		[Required]
		public int NumberOfSeats { get; set; }
	}
}
