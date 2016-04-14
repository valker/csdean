using System;
using System.IO;

namespace csdean
{
    class ProjectFinderFactory : IProjectFinderFactory
    {
        public IProjectFinder Create(string path)
        {
            IProjectFinder finder;
            if (File.Exists(path))
            {
                finder = new SolutionFinder();
            }
            else if (Directory.Exists(path))
            {
                finder = new DirectoryFinder();
            }
            else
            {
                throw new InvalidOperationException("Path should be SLN or directory");
            }

            return finder;
        }
    }
}