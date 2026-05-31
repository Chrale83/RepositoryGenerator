using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class PrimaryKeyTypeIsAttribute<TKey> : Attribute
    {
        public Type PrimaryKeyType => typeof(TKey);
    }
}
