using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DbRepositoryAttribute<T> : Attribute
    {
        public Type Entity { get; } = typeof(T);
    }
}
