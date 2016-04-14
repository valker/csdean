namespace csdean
{
    interface IProjectFinderFactory
    {
        IProjectFinder Create(string path);
    }
}