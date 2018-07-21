using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using System.IO;

namespace DotNetUpdate {
    class Program {

        static async Task<bool> ProcessPackage(NuGetClient nuget, Dependency d) {
            var packages = await nuget.GetPackageInfo(d.Include.ToLower());
            var latest = packages.Versions.First(x => !x.IsPrerelease);
            var latestVersion = latest.ToString();
            if (string.Compare(d.Version, latestVersion) < 0) {
                Console.WriteLine($" Update {d.Include} from {d.Version} to {latestVersion}");
            } else {
                Console.WriteLine($" Skip {d.Include} {d.Version}");
            }
            return true;
        }

        static async Task ProcessProject(string project) {
            Console.WriteLine($"Process project {project}");
            var refs = ProjectParser.GetDependencies(project);
            using (var client = new HttpClient()) {
                var nuget = new NuGetClient(client);
                foreach (var item in refs) {
                    try {
                        _ = await ProcessPackage(nuget, item);
                    } catch (Exception) {
                        Console.WriteLine($" ~ Failed {item.Include}");
                    }
                }
            }
            Console.WriteLine();
        }

        static async Task Main(string[] args) {
            var csproj = Directory.EnumerateFiles(".", "*.csproj", SearchOption.AllDirectories);
            var fsproj = Directory.EnumerateFiles(".", "*.fsproj", SearchOption.AllDirectories);
            var projects = csproj.Concat(fsproj);

            foreach (var item in projects) {
                await ProcessProject(item);
            }
        }
    }
}
