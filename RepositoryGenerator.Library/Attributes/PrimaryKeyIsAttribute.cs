using System;

namespace RepositoryGenerator.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyIsAttribute(string primaryKeyName) : Attribute
    {
        public string PrimaryKeyName => primaryKeyName;
    }
}
