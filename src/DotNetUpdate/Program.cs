using System;
using Buildalyzer;
using Buildalyzer.Workspaces;

namespace DotNetUpdate {
    class Program {
        static void Main(string[] args) {
            var project = @"/Users/wk/Source/DotNetUpdate/tests/MyApp/MyApp.csproj";
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(project);
            var sourcesFiles = analyzer.GetSourceFiles();
            var references = analyzer.GetReferences();

            foreach (var file in sourcesFiles) {
                Console.WriteLine(file);
            }

            foreach (var item in references) {
                Console.WriteLine(item);
            }
            var workspace = analyzer.GetWorkspace();
        }
    }
}
