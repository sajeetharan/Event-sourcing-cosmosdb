using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.WebApi.Infrastructure
{
    public interface IBeforeApplicationStart
    {
        Task OnBeforeApplicationStartAsync();
    }
}
