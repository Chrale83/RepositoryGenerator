using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPClassAttribute<T, DB> : Attribute
    {
        public Type Entity { get; } = typeof(T);
        public Type DbContext { get; } = typeof(DB);
    }
}
