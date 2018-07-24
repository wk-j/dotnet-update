using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace DotNetUpdate {
    public class Dependency {
        public string Include { set; get; }
        public string Version { set; get; }
    }
    public class ProjectParser {
        public static IEnumerable<Dependency> GetDependencies(string project) {
            var xml = File.ReadAllText(project);
            var document = XDocument.Parse(xml);
            var dependencies = document.Descendants("PackageReference")
                .ToList()
                .Select(GetDependency);
            return dependencies;
        }

        private static Dependency GetDependency(XElement element) {
            return new Dependency {
                Include = element.Attribute("Include")?.Value,
                Version = element.Attribute("Version")?.Value
            };
        }
    }
}