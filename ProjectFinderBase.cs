using System.Collections.Generic;

namespace csdean
{
    internal abstract class ProjectFinderBase : IProjectFinder
    {
//        protected IEnumerable<Project> FindProjects(IEnumerable<FileInfo> fileInfos, string rootDirPath)
//        {
//            var rootDirUri = new Uri(rootDirPath);
//            return fileInfos.Select(info => CreateProject(info, rootDirUri)).Where(project => project != null);
//        }
//
//        protected static Project CreateProject(FileInfo projectFileInfo, Uri rootDirUri)
//        {
//            using (TextReader textReader = new StreamReader(projectFileInfo.FullName))
//            {
//                XDocument doc = XDocument.Load(textReader);
//                if (doc.Root == null)
//                {
//                    return null;
//                }
//
//                XNamespace ns = doc.Root.Name.Namespace;
//                string id = doc.Root.Descendants(ns + "ProjectGuid").First().Value.ToUpperInvariant();
//                string assemblyName = doc.Root.Descendants(ns + "AssemblyName").First().Value;
//                var project = new Project
//                {
//                    Path = rootDirUri.MakeRelativeUri(new Uri(projectFileInfo.FullName)).ToString(),
//                    Id = id,
//                    AssemblyName = assemblyName,
//                    Name = projectFileInfo.Name,
//                    References = DirectoryFinder.ParseReferences(doc.Root, ns)
//                };
//
//                return project;
//            }
//        }
//
//        private static Reference[] ParseReferences(XContainer root, XNamespace ns)
//        {
//            var references = new List<Reference>();
//            foreach (XElement reference in root.Descendants(ns + "Reference"))
//            {
//                XAttribute includeAttribute = reference.Attribute("Include");
//                if (reference.Descendants(ns + "HintPath").Any())
//                {
//                    // library
//                    XElement xElement = reference.Element(ns + "HintPath");
//                    if (xElement != null)
//                    {
//                        references.Add(new LibraryReference(includeAttribute.Value, xElement.Value));
//                    }
//                }
//                else
//                {
//                    references.Add(new StandardReference(includeAttribute.Value));
//                }
//            }
//
//            foreach (XElement reference in root.Descendants(ns + "ProjectReference"))
//            {
//                XElement projectElement = reference.Element(ns + "Project");
//                XElement nameElement = reference.Element(ns + "Name");
//                if (projectElement == null || nameElement == null)
//                {
//                    continue;
//                }
//
//                Reference projectReference = new ProjectReference(
//                    reference.Attribute("Include").Value,
//                    projectElement.Value.ToUpperInvariant(), 
//                    nameElement.Value);
//
//                references.Add(projectReference);
//            }
//
//            return references.ToArray();
//        }
//
        public abstract IEnumerable<string> FindProjects(string path);
    }
}