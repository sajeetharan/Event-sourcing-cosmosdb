using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.WebApi
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
            var host = CreateWebHostBuilder(args).Build();
            await host.BeforeApplicationStart();
            host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
