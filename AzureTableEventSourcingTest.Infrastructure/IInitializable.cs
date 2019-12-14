using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
    public interface IInitializable
    {
        Task InitializeAsync();
    }
}
