using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RepositoryGenerator.Library.Models
{
    public class ClassToGenerateData(
        string namespaceName,
        string className,
        string typeArgumentUsingName,
        string typeArgumentName,
        string dbArgumentNamespaceName,
        string dbArgumentName,
        string interfaceName,
        string interfaceUsingNamespace
    ) : IEquatable<ClassToGenerateData>
    {
        public string ClassNamespaceName { get; } = namespaceName;
        public string ClassName { get; } = className;
        public string EntityUsingName { get; } = typeArgumentUsingName;
        public string EntityName { get; } = typeArgumentName;
        public string DbContextUsingName { get; } = dbArgumentNamespaceName;
        public string DbContextName { get; } = dbArgumentName;
        public string InterfaceName { get; } = interfaceName;
        public string InterfaceUsingNamespace { get; } = interfaceUsingNamespace;

        public bool Equals(ClassToGenerateData other)
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
