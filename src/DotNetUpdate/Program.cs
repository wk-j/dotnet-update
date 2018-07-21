using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetUpdate {
    class Program {

        static void Process(string includeVersion, Package package) {
            if (string.Compare(includeVersion, package.Version) < 0) {
                Console.WriteLine($" Update {package.Id} from {includeVersion} {package.Version}");
            } else {
                Console.WriteLine($" Skip {package.Id} {includeVersion}");
            }
        }

        static async Task Main(string[] args) {
            var project = @"/Users/wk/Source/DotNetUpdate/tests/MyApp/MyApp.csproj";
            var refs = ProjectParser.GetDependencies(project);
            using (var client = new HttpClient()) {
                var nuget = new NuGetClient(client);
                foreach (var item in refs) {
                    Console.WriteLine($"Check {item.Include}");
                    var packages = await nuget.GetPackage(item.Include.ToLower());
                    var latest = packages.Last();
                    Process(item.Version, latest);
                }
            }
        }
    }
}
