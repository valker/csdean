using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace csdean
{
    internal abstract class ProjectExtractorBase : IProjectExtractor
    {
        protected IEnumerable<Project> GetProjects(IEnumerable<FileInfo> fileInfos, string path)
        {
            var fromPath = new Uri(path);
            foreach (FileInfo fileInfo in fileInfos)
            {
                using (TextReader textReader = new StreamReader(fileInfo.FullName))
                {
                    XDocument doc = XDocument.Load(textReader);
                    if (doc.Root == null)
                    {
                        continue;
                    }

                    XNamespace ns = doc.Root.Name.Namespace;
                    string id = doc.Root.Descendants(ns + "ProjectGuid").First().Value.ToUpperInvariant();
                    string assemblyName = doc.Root.Descendants(ns + "AssemblyName").First().Value;
                    var projects = new Project
                    {
                        Path = fromPath.MakeRelativeUri(new Uri(fileInfo.FullName)).ToString(),
                        Id = id,
                        AssemblyName = assemblyName,
                        Name = fileInfo.Name,
                        References = DirectoryExtractor.ParseReferences(doc.Root, ns)
                    };

                    yield return projects;
                }
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

            foreach (XElement reference in root.Descendants(ns + "ProjectReference"))
            {
                XElement projectElement = reference.Element(ns + "Project");
                XElement nameElement = reference.Element(ns + "Name");
                if (projectElement == null || nameElement == null)
                {
                    continue;
                }

                Reference projectReference = new ProjectReference(
                    reference.Attribute("Include").Value,
                    projectElement.Value.ToUpperInvariant(), 
                    nameElement.Value);

                references.Add(projectReference);
            }

            return references.ToArray();
        }

        public abstract IEnumerable<Project> GetProjects(string path);
    }
}