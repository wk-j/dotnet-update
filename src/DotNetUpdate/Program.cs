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

            var process = new Process();
            process.StartInfo = new ProcessStartInfo {
                Arguments = args,
                FileName = "dotnet",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            process.Start();
            process.WaitForExit();
        }


        static async Task<bool> ProcessPackage(NuGetClient nuget, Dependency d) {
            var packages = await nuget.GetPackageInfo(d.Include.ToLower());
            var latest = packages.Versions.First(x => !x.IsPrerelease);
            var latestVersion = latest.ToString();
            if (string.Compare(d.Version, latestVersion) < 0) {
                Console.WriteLine($" Update {d.Include} from {d.Version} to {latestVersion}");
                return true;
            } else {
                Console.WriteLine($" Skip {d.Include} {d.Version}");
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
