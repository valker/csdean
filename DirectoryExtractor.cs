using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csdean
{
    internal class DirectoryExtractor : ProjectExtractorBase
    {
        public override IEnumerable<string> GetProjects(string path)
        {
            FileInfo[] fileInfos = new DirectoryInfo(path).GetFiles("*.csproj", SearchOption.AllDirectories);
            return fileInfos.Select(fileInfo => fileInfo.FullName);
        }
    }
}