namespace csdean
{
    public abstract class Reference
    {
        protected Reference(string include)
        {
            Include = include;
        }

        public string Include { get; set; }
    }
}