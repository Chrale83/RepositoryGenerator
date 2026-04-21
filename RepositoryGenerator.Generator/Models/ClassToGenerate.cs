using System;

namespace RepositoryGenerator.Generator.Models
{
    public sealed class ClassToGenerate(
        string namespaceName,
        string className,
        string interfaceName,
        string interfaceUsingNamespace,
        EntityData entityData,
        DbData forClassToGenerate
    ) : IEquatable<ClassToGenerate>
    {
        public string ClassNamespaceName { get; } = namespaceName;
        public string ClassName { get; } = className;
        public string InterfaceName { get; } = interfaceName;
        public string InterfaceUsingNamespace { get; } = interfaceUsingNamespace;
        public DbData DbForClassToGenerate { get; } = forClassToGenerate;
        public EntityData EntityData { get; } = entityData;

        public override bool Equals(object? obj) => Equals(obj as ClassToGenerate);

        public bool Equals(ClassToGenerate? other)
        {
            if (other is null)
                return false;
            return ClassNamespaceName == other.ClassNamespaceName
                && ClassName == other.ClassName
                && InterfaceName == other.InterfaceName
                && InterfaceUsingNamespace == other.InterfaceUsingNamespace
                && EntityData == other.EntityData
                && DbForClassToGenerate == other.DbForClassToGenerate;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (ClassNamespaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (ClassName?.GetHashCode() ?? 0);
                hash = hash * 31 + (InterfaceName?.GetHashCode() ?? 0);
                hash = hash * 31 + (InterfaceUsingNamespace?.GetHashCode() ?? 0);
                hash = hash * 31 + (EntityData?.GetHashCode() ?? 0);
                hash = hash * 31 + (DbForClassToGenerate?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    public sealed class DbData(string dbSetName, string dbArgumentName, string dbContextUsingName)
        : IEquatable<DbData>
    {
        public string DbSetName { get; } = dbSetName;
        public string DbArgumentName { get; } = dbArgumentName;
        public string DbContextUsingName { get; } = dbContextUsingName;

        public bool Equals(DbData? other)
        {
            if (other is null)
                return false;
            return DbSetName == other.DbSetName
                && DbArgumentName == other.DbArgumentName
                && DbContextUsingName == other.DbContextUsingName;
        }

        public override bool Equals(object? obj) => Equals(obj as DbData);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (DbSetName?.GetHashCode() ?? 0);
                hash = hash * 31 + (DbArgumentName?.GetHashCode() ?? 0);
                hash = hash * 31 + (DbContextUsingName?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    public sealed class EntityData(
        string entityUsingName,
        string entityName,
        string entityPrimaryKey
    ) : IEquatable<EntityData>
    {
        public string EntityUsingName { get; } = entityUsingName;
        public string EntityName { get; } = entityName;
        public string EntityPrimaryKey { get; } = entityPrimaryKey;

        public bool Equals(EntityData? other)
        {
            if (other is null)
                return false;
            return EntityUsingName == other.EntityUsingName
                && EntityName == other.EntityName
                && EntityPrimaryKey == other.EntityPrimaryKey;
        }

        public override bool Equals(object? obj) => Equals(obj as EntityData);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (EntityUsingName?.GetHashCode() ?? 0);
                hash = hash * 31 + (EntityName?.GetHashCode() ?? 0);
                hash = hash * 31 + (EntityPrimaryKey?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
