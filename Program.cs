using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CsProjDependencyBuilder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = args[0];
            FileInfo[] fileInfos = new DirectoryInfo(path).GetFiles("*.csproj", SearchOption.AllDirectories);
            Project[] projects = GetProjects(fileInfos, path).ToArray();
            Dictionary<string, Project> projs = projects.ToDictionary(project => project.Id, project => project);
            var wannaExit = false;
            while (!wannaExit)
            {
                Console.Write(">");
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "exit":
                        wannaExit = true;
                        break;
                    case "listbyid":
                        Console.WriteLine(string.Join("\n",
                            projects.OrderBy(project => project.Id).Select(project => project.ToString())));
                        break;
                    case "listbyname":
                        Console.WriteLine(string.Join("\n",
                            projects.OrderBy(project => project.Name).Select(project => project.ToString())));
                        break;
                    case "listbyassembly":
                        Console.WriteLine(string.Join("\n",
                            projects.OrderBy(project => project.AssemblyName).Select(project => project.ToString())));
                        break;
                    case "listbyrefcount":
                        Console.WriteLine(string.Join("\n",
                            projects.OrderBy(project => project.References.Length).Select(project => project.ToString())));
                        break;
                    case "listbydepcount":
                        Console.WriteLine(string.Join("\n",
                            projects.OrderBy(project => GetDependencies(project, projects).Count())
                                .Select(project => project.ToString())));
                        break;
                    case "b":
                        BuildGraph(projects);
                        break;
                    default:
                    {
                        string[] parts = cmd.Split(' ');
                        if (parts.Length < 1)
                        {
                            throw new InvalidOperationException("wrong command format");
                        }
                    }
                        break;
                }
            }
        }

        private static void BuildGraph(Project[] projects)
        {
//            projects = new[]
//            {
//                new Project
//                {
//                    AssemblyName = "A1",
//                    Name = "N1",
//                    Path = "P1",
//                    Id = "1",
//                    References = new Reference[] {new ProjectReference("i1", "2", "N2")}
//                },
//                new Project {AssemblyName = "A2", Name = "N2", Path = "P2", Id = "2", References = new Reference[0]}
//            };

            XNamespace xmlns = "http://graphml.graphdrawing.org/xmlns";
            XNamespace java = "http://www.yworks.com/xml/yfiles-common/1.0/java";
            XNamespace sys = "http://www.yworks.com/xml/yfiles-common/markup/primitives/2.0";
            XNamespace x = "http://www.yworks.com/xml/yfiles-common/markup/2.0";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace y = "http://www.yworks.com/xml/graphml";
            XNamespace yed = "http://www.yworks.com/xml/yed/3";
            XNamespace schemaLocation =
                "http://graphml.graphdrawing.org/xmlns http://www.yworks.com/xml/schema/graphml/1.1/ygraphml.xsd";

            var graphElement = new XElement(xmlns + "graph", new XAttribute("edgedefault", "directed"),
                new XAttribute("id", "G"), new XElement(xmlns + "data", new XAttribute("key", "d0")));

            var idMap = new Dictionary<string, string>();
            var n = 0;
            foreach (Project project in projects)
            {
                string id = string.Format("n{0}", n);
                var nodeElement = new XElement(xmlns + "node", new XAttribute("id", id),
                    new XElement(xmlns + "data", new XAttribute("key", "d5")),
                    new XElement(xmlns + "data", new XAttribute("key", "d6"),
                        new XElement(y + "ShapeNode",
                            new XElement(y + "Geometry", new XAttribute("height", "30.0"),
                                new XAttribute("width", "30.0")),
                            new XElement(y + "Fill", new XAttribute("color", "#FFCC00"),
                                new XAttribute("transparent", "false")),
                            new XElement(y + "BorderStyle", new XAttribute("color", "#000000"),
                                new XAttribute("type", "line"), new XAttribute("width", "1.0")),
                            new XElement(y + "NodeLabel",
                                new XAttribute("alignment", "center"),
                                new XAttribute("autoSizePolicy", "content"),
                                new XAttribute("fontFamily", "Dialog"),
                                new XAttribute("fontSize", "12"),
                                new XAttribute("fontStyle", "plain"),
                                new XAttribute("hasBackgroundColor", "false"),
                                new XAttribute("hasLineColor", "false"),
                                new XAttribute("height", "18.0"),
                                new XAttribute("modelName", "custom"),
                                new XAttribute("textColor", "#000000"),
                                new XAttribute("visible", "true"),
                                new XAttribute("width", "10.0"),
                                project.AssemblyName,
                                new XElement(y + "LabelModel",
                                    new XElement(y + "SmartNodeLabelModel", new XAttribute("distance", "4.0"))),
                                new XElement(y + "ModelParameter",
                                    new XElement(y + "SmartNodeLabelModelParameter",
                                        new XAttribute("labelRatioX", "0.0"),
                                        new XAttribute("labelRatioY", "0.0"), new XAttribute("nodeRatioX", "0.0"),
                                        new XAttribute("nodeRatioY", "0.0"), new XAttribute("offsetX", "0.0"),
                                        new XAttribute("offsetY", "0.0"), new XAttribute("upX", "0.0"),
                                        new XAttribute("upY", "-1.0")))),
                            new XElement(y + "Shape", new XAttribute("type", "rectangle")))));
                graphElement.Add(nodeElement);

                idMap.Add(project.Id, id);
                n++;
            }


            n = 0;
            foreach (Project project in projects)
            {
                foreach (ProjectReference projectReference in project.References.OfType<ProjectReference>())
                {
                    string sourceId;
                    string targetId;
                    bool sourceIdDefined = idMap.TryGetValue(project.Id, out sourceId);
                    bool targetIdDefined = idMap.TryGetValue(projectReference.Id, out targetId);

                    if (sourceIdDefined && targetIdDefined)
                    {
                        string id = string.Format("e{0}", n);
                        var edgeElement = new XElement(xmlns + "edge", new XAttribute("id", id),
                            new XAttribute("source", sourceId),
                            new XAttribute("target", targetId),
                            new XElement(xmlns + "data", new XAttribute("key", "d9")),
                            new XElement(xmlns + "data", new XAttribute("key", "d10"),
                                new XElement(y + "PolyLineEdge",
                                    new XElement(y + "Path", new XAttribute("sx", "0.0"), new XAttribute("sy", "0.0"),
                                        new XAttribute("tx", "0.0"), new XAttribute("ty", "0.0")),
                                    new XElement(y + "LineStyle", new XAttribute("color", "#000000"),
                                        new XAttribute("type", "line"), new XAttribute("width", "1.0")),
                                    new XElement(y + "Arrows", new XAttribute("source", "none"),
                                        new XAttribute("target", "standard")),
                                    new XElement(y + "BendStyle", new XAttribute("smoothed", "false")))));

                        graphElement.Add(edgeElement);
                        n++;
                    }
                }
            }

            var root = new XElement(xmlns + "graphml",
                new XAttribute("xmlns", xmlns),
                new XAttribute(XNamespace.Xmlns + "java", java),
                new XAttribute(XNamespace.Xmlns + "sys", sys),
                new XAttribute(XNamespace.Xmlns + "x", x),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "y", y),
                new XAttribute(XNamespace.Xmlns + "yed", yed),
                new XAttribute(xsi + "schemaLocation", schemaLocation),
                new XElement(xmlns + "key", new XAttribute("attr.name", "Description"),
                    new XAttribute("attr.type", "string"), new XAttribute("for", "graph"), new XAttribute("id", "d0")),
                new XElement(xmlns + "key", new XAttribute("for", "port"), new XAttribute("id", "d1"),
                    new XAttribute("yfiles.type", "portgraphics")),
                new XElement(xmlns + "key", new XAttribute("for", "port"), new XAttribute("id", "d2"),
                    new XAttribute("yfiles.type", "portgeometry")),
                new XElement(xmlns + "key", new XAttribute("for", "port"), new XAttribute("id", "d3"),
                    new XAttribute("yfiles.type", "portuserdata")),
                new XElement(xmlns + "key", new XAttribute("attr.name", "url"), new XAttribute("attr.type", "string"),
                    new XAttribute("for", "node"), new XAttribute("id", "d4")),
                new XElement(xmlns + "key", new XAttribute("attr.name", "description"),
                    new XAttribute("attr.type", "string"), new XAttribute("for", "node"), new XAttribute("id", "d5")),
                new XElement(xmlns + "key", new XAttribute("for", "node"), new XAttribute("yfiles.type", "nodegraphics"),
                    new XAttribute("id", "d6")),
                new XElement(xmlns + "key", new XAttribute("for", "graphml"), new XAttribute("yfiles.type", "resources"),
                    new XAttribute("id", "d7")),
                new XElement(xmlns + "key", new XAttribute("for", "edge"), new XAttribute("id", "d8"),
                    new XAttribute("attr.name", "url"), new XAttribute("attr.type", "string")),
                new XElement(xmlns + "key", new XAttribute("for", "edge"), new XAttribute("id", "d9"),
                    new XAttribute("attr.name", "description"), new XAttribute("attr.type", "string")),
                new XElement(xmlns + "key", new XAttribute("for", "edge"), new XAttribute("id", "d10"),
                    new XAttribute("yfiles.type", "edgegraphics")),
                graphElement,
                new XElement(xmlns + "data", new XAttribute("key", "d7"), new XElement(y + "Resources"))
                );

            root.Save("graph.graphml");
        }

        private static IEnumerable<Project> GetDependencies(Project project, Project[] projects)
        {
            return
                projects.Where(
                    project1 =>
                        project1.References.OfType<ProjectReference>().Any(reference => reference.Id == project.Id));
        }

        private static void ShowHelp()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Project> GetProjects(FileInfo[] fileInfos, string path)
        {
            var fromPath = new Uri(path);
            foreach (FileInfo fileInfo in fileInfos)
            {
                using (TextReader textReader = new StreamReader(fileInfo.FullName))
                {
                    XDocument doc = XDocument.Load(textReader);
                    XNamespace ns = doc.Root.Name.Namespace;
                    string id = doc.Root.Descendants(ns + "ProjectGuid").First().Value.ToUpperInvariant();
                    string assemblyName = doc.Root.Descendants(ns + "AssemblyName").First().Value;
                    var projects = new Project
                    {
                        Path = fromPath.MakeRelativeUri(new Uri(fileInfo.FullName)).ToString(),
                        Id = id,
                        AssemblyName = assemblyName,
                        Name = fileInfo.Name,
                        References = ParseReferences(doc.Root, ns)
                    };
                    yield return projects;
                }
            }
        }

        private static Reference[] ParseReferences(XElement root, XNamespace ns)
        {
            var references = new List<Reference>();
            foreach (XElement reference in root.Descendants(ns + "Reference"))
            {
                XAttribute includeAttribute = reference.Attribute("Include");
                if (reference.Descendants(ns + "HintPath").Any())
                {
                    // library
                    references.Add(new LibraryReference(includeAttribute.Value,
                        reference.Element(ns + "HintPath").Value));
                }
                else
                {
                    references.Add(new StandardReference(includeAttribute.Value));
                }
            }

            foreach (XElement reference in root.Descendants(ns + "ProjectReference"))
            {
                references.Add(new ProjectReference(reference.Attribute("Include").Value,
                    reference.Element(ns + "Project").Value.ToUpperInvariant(), reference.Element(ns + "Name").Value));
            }


            return references.ToArray();
        }
    }
}