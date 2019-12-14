using AzureTableEventSourcingTest.Domain;

namespace AzureTableEventSourcingTest.Infrastructure
{
    public class EventRecord<TId>
	{
		public EventRecord(TId aggregateRootId, int versionNumber, IEvent @event)
		{
            AggregateRootId = aggregateRootId;
            VersionNumber = versionNumber;
            Event = @event;
		}
		
        public TId AggregateRootId { get; }
        public int VersionNumber { get; }
		public IEvent Event { get; }
	}
}
