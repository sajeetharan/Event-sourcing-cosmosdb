using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
    public static class AssemblyExtensions
    {
        public static async Task<string> ReadEmbeddedResourceAsStringAsync(this Assembly assembly, string path)
        {
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{path}"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
