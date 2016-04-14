using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace csdean
{
    class ProjectFactory : IProjectFactory
    {
        public Project Create(string projectPath)
        {
            using (TextReader reader = new StreamReader(projectPath))
            {
                XDocument doc = XDocument.Load(reader, LoadOptions.None);

                if (doc.Root == null) return null;
                XNamespace ns = doc.Root.Name.Namespace;
                string id = doc.Root.Descendants(ns + "ProjectGuid").First().Value.ToUpperInvariant();
                string assemblyName = doc.Root.Descendants(ns + "AssemblyName").First().Value;
                var project = new Project
                {
                    Path = projectPath,
                    Id = id,
                    AssemblyName = assemblyName,
                    Name = Path.GetFileNameWithoutExtension(projectPath),
                    References = ParseReferences(doc.Root, ns)
                };

                return project;
            }
        }

        private static Reference[] ParseReferences(XContainer root, XNamespace ns)
        {
            var references = new List<Reference>();
            foreach (XElement reference in root.Descendants(ns + "Reference"))
            {
                XAttribute includeAttribute = reference.Attribute("Include");
                if (reference.Descendants(ns + "HintPath").Any())
                {
                    // library
                    XElement xElement = reference.Element(ns + "HintPath");
                    if (xElement != null)
                    {
                        references.Add(new LibraryReference(includeAttribute.Value, xElement.Value));
                    }
                }
                else
                {
                    references.Add(new StandardReference(includeAttribute.Value));
                }
            }

            foreach (XElement reference in root.Descendants(ns+"ProjectReference"))
            {
                XAttribute include = reference.Attribute("Include");
                XElement project = reference.Descendants(ns + "Project").First();
                XElement name = reference.Descendants(ns + "Name").First();
                references.Add(new ProjectReference(include.Value, project.Value.ToUpperInvariant(), name.Value));
            }

            return references.ToArray();
        }
    }
}