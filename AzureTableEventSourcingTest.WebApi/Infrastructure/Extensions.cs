using AzureTableEventSourcingTest.Domain;
using AzureTableEventSourcingTest.Infrastructure;
using AzureTableEventSourcingTest.WebApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddInitializable<TService>(this IServiceCollection services)
            where TService: IInitializable
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IBeforeApplicationStart, InitializeBeforeApplicationStart>(sp 
                => new InitializeBeforeApplicationStart(sp.GetRequiredService<TService>())));
            return services;
        }

        public static IServiceCollection AddCosmosDbEventStore<TId, TAggregateRoot>(this IServiceCollection services)
            where TAggregateRoot : IAggregateRoot<TId> 
            => services
                .AddTransient<CosmosDbEventStore<TId, TAggregateRoot>>()
                .AddTransient<IEventStore<TId, TAggregateRoot>>(sp => sp.GetRequiredService<CosmosDbEventStore<TId, TAggregateRoot>>())
                .AddInitializable<CosmosDbEventStore<TId, TAggregateRoot>>();
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class Extensions
    {
        public static async Task BeforeApplicationStart(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IBeforeApplicationStart>();
                await Task.WhenAll(handlers.Select(x => x.OnBeforeApplicationStartAsync()));
            }
        }
    }
}
