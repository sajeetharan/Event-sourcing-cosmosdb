using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Domain
{
	public interface ICommandHandler<in TCommand, TResult>
	{
		Task<TResult> ExecuteAsync(TCommand command);
	}
}
