using System.Collections.Generic;

namespace csdean
{
    internal interface IProjectExtractor
    {
        IEnumerable<Project> GetProjects(string path);
    }
}