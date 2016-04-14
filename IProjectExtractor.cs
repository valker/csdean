using System.Collections.Generic;

namespace csdean
{
    internal interface IProjectExtractor
    {
        IEnumerable<string> GetProjects(string path);
    }
}