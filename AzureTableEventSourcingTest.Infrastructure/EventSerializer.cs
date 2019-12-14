using AzureTableEventSourcingTest.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using System.Text;

namespace AzureTableEventSourcingTest.Infrastructure
{
	public static class EventSerializer
	{
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings;
        public static readonly JsonSerializer DefaultJsonSerializer;

		static EventSerializer()
		{
            DefaultJsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Converters = {
                    new StringEnumConverter(camelCaseText: true),
                },
            };
            DefaultJsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            DefaultJsonSerializer = JsonSerializer.Create(DefaultJsonSerializerSettings);
        }

		public static string ToString(IEvent @event)
			=> JsonConvert.SerializeObject(@event, DefaultJsonSerializerSettings);

		public static byte[] ToBytes(IEvent @event) 
			=> DefaultEncoding.GetBytes(ToString(@event));

        public static JToken ToJson(IEvent @event)
            => JToken.FromObject(@event, DefaultJsonSerializer);

		public static IEvent FromString(string json)
			=> JsonConvert.DeserializeObject<IEvent>(json, DefaultJsonSerializerSettings);

		public static IEvent FromBytes(byte[] bytes)
			=> FromString(DefaultEncoding.GetString(bytes));

        public static IEvent FromJson(JToken json)
            => json.ToObject<IEvent>(DefaultJsonSerializer);
	}
}
