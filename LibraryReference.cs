namespace CsProjDependencyBuilder
{
    public class LibraryReference : Reference
    {
        public LibraryReference(string include, string hintPath) : base(include)
        {
            HintPath = hintPath;
        }

        public string HintPath { get; set; }
    }
}