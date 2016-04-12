using System.Collections.Generic;

namespace CsProjDependencyBuilder
{
    internal interface IProjectExtractor
    {
        IEnumerable<Project> GetProjects(string path);
    }
}