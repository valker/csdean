namespace CsProjDependencyBuilder
{
    public class ProjectReference : Reference
    {
        public ProjectReference(string include, string id, string name) : base(include)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}