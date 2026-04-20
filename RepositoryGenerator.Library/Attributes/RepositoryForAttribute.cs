using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryForAttribute<T, DB> : Attribute
    {
        public Type Entity { get; } = typeof(T);
        public Type DbContext { get; } = typeof(DB);
    }
}
