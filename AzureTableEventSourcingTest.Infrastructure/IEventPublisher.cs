using AzureTableEventSourcingTest.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
	public interface IEventPublisher
	{
		Task PublishAsync(IEnumerable<IEvent> events);
	}

	public class NullEventPublisher : IEventPublisher
	{
		public Task PublishAsync(IEnumerable<IEvent> events)
			=> Task.CompletedTask;
	}
}
