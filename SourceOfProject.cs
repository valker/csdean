namespace csdean
{
    interface SourceOfProject
    {
        Project[] GetFrom(string path);
    }
}