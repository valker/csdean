using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CsProjDependencyBuilder
{
    internal class SolutionExtractor : ProjectExtractorBase
    {
        private static readonly char[] Separators = {' ', '"'};
        public override IEnumerable<Project> GetProjects(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line != null && line.StartsWith("Project"))
                    {
                        string[] first = line.Split('=');
                        string[] second = first[1].Split(',');
                        if (second[1].Contains(".csproj"))
                        {
                            string filename = second[1].Trim(Separators);
                            string directory = Path.GetDirectoryName(path);
                            string fullPath = Path.Combine(directory, filename);
                            this.
                            Debug.WriteLine(second[1]);
                        }
                    }
                }
                yield break;
            }
        }
    }
}