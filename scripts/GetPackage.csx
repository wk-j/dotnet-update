#! "netcoreapp2.1"
#r "../src/DotNetUpdate/bin/Debug/netcoreapp2.1/DotNetUpdate.dll"
#r "nuget:Newtonsoft.Json,11.0.0"

using DotNetUpdate;
using System.Net.Http;

var client = new HttpClient();
var nuget = new NuGetClient(client);
var package = await nuget.GetPackage("bcircle.cmmdeploy");

foreach (var item in package) {
    Console.WriteLine(item);
}