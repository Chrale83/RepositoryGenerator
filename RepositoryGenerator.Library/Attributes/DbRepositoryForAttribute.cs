using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbRepositoryForAttribute<T, TDbContext> : Attribute
        where T : class
        where TDbContext : class
    {
        public Type Entity => typeof(T);
        public Type DbContext => typeof(TDbContext);
    }
}
