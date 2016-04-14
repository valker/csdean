using System.Linq;

namespace csdean
{
    internal class DefaultSourceOfProject : SourceOfProject {
        private readonly IProjectFinderFactory _projectFinderFactory;
        private readonly IProjectFactory _projectFactory;

        public DefaultSourceOfProject(IProjectFinderFactory projectFinderFactory, IProjectFactory projectFactory)
        {
            _projectFinderFactory = projectFinderFactory;
            _projectFactory = projectFactory;
        }

        public Project[] GetFrom(string path)
        {
            IProjectFinder finder = _projectFinderFactory.Create(path);
            string[] projects = finder.FindProjects(path).ToArray();
            Project[] projectsInfo = projects.Select(s => _projectFactory.Create(s)).ToArray();

            return projectsInfo;
        }
    }
}