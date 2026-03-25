using System;

namespace RepositoryGenerator.Generator.Models
{
    public class ClassToGenerate(
        string namespaceName,
        string className,
        string entityUsingName,
        string entityName,
        string dbArgumentNamespaceName,
        string dbArgumentName,
        string interfaceName,
        string interfaceUsingNamespace,
        string entityPrimaryKey,
        string dbSetName
    ) : IEquatable<ClassToGenerate>
    {
        public string ClassNamespaceName { get; } = namespaceName;
        public string ClassName { get; } = className;
        public string EntityUsingName { get; } = entityUsingName;
        public string EntityName { get; } = entityName;
        public string DbContextUsingName { get; } = dbArgumentNamespaceName;
        public string DbContextName { get; } = dbArgumentName;
        public string InterfaceName { get; } = interfaceName;
        public string InterfaceUsingNamespace { get; } = interfaceUsingNamespace;
        public string EntityPrimaryKey { get; } = entityPrimaryKey;
        public string DbSetName { get; } = dbSetName;

        public bool Equals(ClassToGenerate other)
        {
            if (other is null)
                return false;
            return ClassNamespaceName == other.ClassNamespaceName
                && ClassName == other.ClassName
                && EntityUsingName == other.EntityUsingName
                && EntityName == other.EntityName
                && DbContextUsingName == other.DbContextUsingName
                && DbContextName == other.DbContextName
                && InterfaceName == other.InterfaceName
                && InterfaceUsingNamespace == other.InterfaceUsingNamespace;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (ClassNamespaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (ClassName?.GetHashCode() ?? 0);
                hash = hash * 31 + (EntityUsingName?.GetHashCode() ?? 0);
                hash = hash * 31 + (EntityName?.GetHashCode() ?? 0);
                hash = hash * 31 + (DbContextUsingName?.GetHashCode() ?? 0);
                hash = hash * 31 + (DbContextName?.GetHashCode() ?? 0);
                hash = hash * 31 + (InterfaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (InterfaceUsingNamespace?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
