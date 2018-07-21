using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DotNetUpdate {
    public class NuGetClient {
        private HttpClient httpClient;
        public NuGetClient(HttpClient client) {
            httpClient = client;
        }

        private async Task<JObject> GetPackageObject(string name) {
            var response = await httpClient.GetAsync($"http://api.nuget.org/v3/registration3/{name}/index.json");
            if (response.IsSuccessStatusCode) {
                return JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<IEnumerable<Package>> GetPackage(string name) {
            var obj = await GetPackageObject(name);
            var items = obj["items"][0]["items"];
            var data = items.Select(x => {
                var v = (string)x["catalogEntry"]["version"];
                return new Package {
                    Version = v,
                    Id = (string)x["catalogEntry"]["id"]
                };
            });
            return data;
        }
    }
}