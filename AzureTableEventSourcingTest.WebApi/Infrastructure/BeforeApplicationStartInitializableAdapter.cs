using AzureTableEventSourcingTest.Infrastructure;
using System;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.WebApi.Infrastructure
{
    public class BeforeApplicationStartInitializableAdapter : IBeforeApplicationStart
    {
        private readonly IInitializable initializable;

        public BeforeApplicationStartInitializableAdapter(IInitializable initializable)
        {
            this.initializable = initializable ?? throw new ArgumentNullException(nameof(initializable));
        }

        public async Task OnBeforeApplicationStartAsync()
            => await initializable.InitializeAsync();
    }
}
