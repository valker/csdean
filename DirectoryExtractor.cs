using System.Collections.Generic;
using System.IO;

namespace CsProjDependencyBuilder
{
    internal class DirectoryExtractor : ProjectExtractorBase
    {
        public override IEnumerable<Project> GetProjects(string path)
        {
            FileInfo[] fileInfos = new DirectoryInfo(path).GetFiles("*.csproj", SearchOption.AllDirectories);
            return GetProjects(fileInfos, path);
        }
    }
}