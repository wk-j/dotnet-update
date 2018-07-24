using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using System.IO;
using System.Diagnostics;

namespace DotNetUpdate {
    class Program {

        static void Update(string project, string package) {
            var args = $"add {project} package {package}";
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    Arguments = args,
                    FileName = "dotnet",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();
        }

        static async Task<bool> ProcessPackage(NuGetClient nuget, Dependency dependency) {
            var packages = await nuget.GetPackageInfo(dependency.Include);
            var latest = packages.Versions.First(x => !x.IsPrerelease);
            var latestVersion = latest.ToString();
            if (string.Compare(dependency.Version, latestVersion) < 0 && !String.IsNullOrEmpty(dependency.Version)) {
                Console.WriteLine($" Update {dependency.Include} from {dependency.Version} to {latestVersion}");
                return true;
            } else {
                Console.WriteLine($" Skip {dependency.Include} {dependency.Version}");
                return false;
            }
        }

        static async Task ProcessProject(string project) {
            Console.WriteLine($"Process project {project}");
            var refs = ProjectParser.GetDependencies(project);
            using (var client = new HttpClient()) {
                var nuget = new NuGetClient(client);
                foreach (var item in refs) {
                    try {
                        var update = await ProcessPackage(nuget, item);
                        if (update) {
                            Update(project, item.Include);
                        }
                    } catch (Exception e) {
                        Console.WriteLine($" ~ Failed {item.Include}");
                        Console.WriteLine($" ~ {e.Message}");
                        Console.WriteLine($" ~ {e.StackTrace}");
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
