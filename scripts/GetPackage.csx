#! "netcoreapp2.1"
#r "../src/DotNetUpdate/bin/Debug/netcoreapp2.1/DotNetUpdate.dll"
#r "nuget:Newtonsoft.Json,11.0.0"
#r "nuget:NuGet.Versioning, 4.7.0"

using DotNetUpdate;
using System.Net.Http;
using NuGet.Versioning;

var client = new HttpClient();
var nuget = new NuGetClient(client);
var package = await nuget.GetPackageInfo("BCircle.CMMDeploy");

foreach (var item in package.Versions) {
    Console.WriteLine(item);
}