using System.Collections.Generic;

namespace csdean
{
    internal interface IProjectFinder
    {
        IEnumerable<string> FindProjects(string path);
    }
}