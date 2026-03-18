using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPInterfaceAttribute<T> : Attribute
    {
        public Type Entity { get; } = typeof(T);
    }
}
