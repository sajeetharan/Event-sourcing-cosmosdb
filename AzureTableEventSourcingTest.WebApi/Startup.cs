using AzureTableEventSourcingTest.Domain;
using AzureTableEventSourcingTest.Domain.Flights;
using AzureTableEventSourcingTest.Domain.Flights.Commands;
using AzureTableEventSourcingTest.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace AzureTableEventSourcingTest.WebApi
{
    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new Info
				{
					Title = "API",
					Version = "v1",
				});
				options.MapType<IataAirportCode>(() => new Schema { Type = "string" });
				options.MapType<LocalTime>(() => new Schema { Type = "string" });
			});

			services
				.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.Configure<MvcJsonOptions>(options =>
			{
				options.SerializerSettings.ContractResolver = new DefaultContractResolver()
				{
					NamingStrategy = new CamelCaseNamingStrategy(),
				};
				options.SerializerSettings.Converters.Add(new StringEnumConverter(camelCaseText: true));
				options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
			});

            services
				.AddTransient<ICommandHandler<CreateFlight.Command, CreateFlight.Result>, CreateFlight>()
				.AddTransient<ICommandHandler<AllotFlightSeats.Command, AllotFlightSeats.Result>, AllotFlightSeats>()
				.AddTransient<ICommandHandler<BookFlightSeats.Command, BookFlightSeats.Result>, BookFlightSeats>()
                .AddTransient(_ => new DocumentClient(new Uri(Configuration["Azure:CosmosDb:AccountEndpoint"]), Configuration["Azure:CosmosDb:AccountKey"]))
                .AddSingleton(new CosmosDbEventStoreSettings("default"))
				.AddCosmosDbEventStore<FlightId, Flight>();
		}
		
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseHttpsRedirection();
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
			});
			app.UseMvc();
		}
	}
}
