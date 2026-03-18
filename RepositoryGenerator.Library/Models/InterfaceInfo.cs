using System;
using System.Collections.Generic;

namespace RepositoryGenerator.Library.Models
{
    public sealed class InterfaceInfo : IEquatable<InterfaceInfo>
    {
        public InterfaceInfo(
            string interfaceNamespaceName,
            string interfaceName,
            string argumentName,
            string argumentUsingName
        )
        {
            NamespaceName = interfaceNamespaceName;
            InterfaceName = interfaceName;
            ArgumentName = argumentName;
            ArgumentUsingName = argumentUsingName;
        }

        public string NamespaceName { get; }
        public string InterfaceName { get; }
        public string ArgumentName { get; }
        public string ArgumentUsingName { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as InterfaceInfo);
        }

        public bool Equals(InterfaceInfo? other)
        {
            return other is not null
                && NamespaceName == other.NamespaceName
                && InterfaceName == other.InterfaceName
                && ArgumentName == other.ArgumentName
                && ArgumentUsingName == other.ArgumentUsingName;
        }

        public override int GetHashCode()
        {
            int hashCode = -39551721;
            hashCode =
                hashCode * -1521134295
                + EqualityComparer<string>.Default.GetHashCode(NamespaceName);
            hashCode =
                hashCode * -1521134295
                + EqualityComparer<string>.Default.GetHashCode(InterfaceName);
            return hashCode;
        }
    }
}
