using System.Text;

namespace CsProjDependencyBuilder
{
    public class Project
    {
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public string Path { get; set; }
        public string Id { get; set; }
        public Reference[] References { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Project:(");
            sb.Append(Name);
            sb.Append(',');
            sb.Append(AssemblyName);
            sb.Append(',');
            sb.Append(References.Length);
            sb.Append(')');
            return sb.ToString();
        }
    }
}