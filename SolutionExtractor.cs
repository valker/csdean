﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csdean
{
    internal class SolutionExtractor : ProjectExtractorBase
    {
        private static readonly char[] Separators = {' ', '"'};

        public override IEnumerable<Project> GetProjects(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            if (directoryName == null) return Enumerable.Empty<Project>();
            var rootDirectoryUri = new Uri(directoryName);
            return GetProjectPaths(path).Select(projectPath => CreateProject(new FileInfo(projectPath), rootDirectoryUri));
        }

        private static IEnumerable<string> GetProjectPaths(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null || !line.StartsWith("Project")) continue;
                    string[] first = line.Split('=');
                    string[] second = first[1].Split(',');
                    if (!second[1].Contains(".csproj")) continue;
                    string referencedProjectRelativePath = second[1].Trim(Separators);
                    string projectDirectoryName = Path.GetDirectoryName(path);
                    if (projectDirectoryName == null) continue;
                    string referencedProjectullPath = Path.Combine(projectDirectoryName, referencedProjectRelativePath);
                    yield return referencedProjectullPath;
                }
            }
        }
    }
}