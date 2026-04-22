using System;

namespace RepositoryGenerator.Generator.Models
{
    public sealed class InterfaceToGenerate(
        string interfaceNamespaceName,
        string interfaceName,
        string argumentName,
        string argumentUsingName,
        string primaryKeyType
    ) : IEquatable<InterfaceToGenerate>
    {
        public string NamespaceName { get; } = interfaceNamespaceName;
        public string InterfaceName { get; } = interfaceName;
        public string ArgumentName { get; } = argumentName;
        public string ArgumentUsingName { get; } = argumentUsingName;
        public string PrimaryKeyType { get; } = primaryKeyType;

        public override bool Equals(object? obj)
        {
            return Equals(obj as InterfaceToGenerate);
        }

        public bool Equals(InterfaceToGenerate? other)
        {
            return other is not null
                && NamespaceName == other.NamespaceName
                && InterfaceName == other.InterfaceName
                && ArgumentName == other.ArgumentName
                && ArgumentUsingName == other.ArgumentUsingName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (NamespaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (InterfaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (ArgumentName?.GetHashCode() ?? 0);
                hash = hash * 31 + (ArgumentUsingName?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
