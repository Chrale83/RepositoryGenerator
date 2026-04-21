using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RepositoryForAttribute<T> : Attribute
        where T : class
    {
        public Type Entity { get; } = typeof(T);
    }
}
