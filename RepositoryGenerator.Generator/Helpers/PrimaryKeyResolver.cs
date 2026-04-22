using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RepositoryGenerator.Generator.Helpers
{
    internal static class PrimaryKeyResolver
    {
        private const string PrimaryKeyAttribute =
            "RepositoryGenerator.Library.Attributes.PrimaryKeyIsAttribute";

        public static string? ResolvePrimaryKeyName(
            GeneratorSyntaxContext context,
            ITypeSymbol entitySymbol,
            INamedTypeSymbol classSymbol
        )
        {
            var entityName = entitySymbol.Name;
            var primaryKeyAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                PrimaryKeyAttribute
            );

            if (primaryKeyAttributeSymbol is not null)
            {
                var primaryKeyAttribute = classSymbol
                    .GetAttributes()
                    .FirstOrDefault(a =>
                        SymbolEqualityComparer.Default.Equals(
                            a.AttributeClass?.OriginalDefinition,
                            primaryKeyAttributeSymbol
                        )
                    );
                if (primaryKeyAttribute is not null)
                {
                    return primaryKeyAttribute
                            .ConstructorArguments.FirstOrDefault()
                            .Value?.ToString() ?? string.Empty;
                }
                else
                {
                    var conventionPrimaryKey = $"{entityName}Id";
                    var props = entitySymbol
                        .GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(p =>
                            string.Equals(
                                p.Name,
                                conventionPrimaryKey,
                                StringComparison.OrdinalIgnoreCase
                            ) || string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                    if (props.Count == 1 && props[0].Type.SpecialType == SpecialType.System_Int32)
                    {
                        return props[0].Name;
                    }
                }
            }
            return null;
        }

        public static string ResolvePrimaryKeyType(
            GeneratorSyntaxContext context,
            INamedTypeSymbol interfaceTypeSymbol
        )
        {
            var primaryKeyTypeAttributeSymbol =
                context.SemanticModel.Compilation.GetTypeByMetadataName(
                    "RepositoryGenerator.Library.Attributes.PrimaryKeyTypeIsAttribute`1"
                );
            if (primaryKeyTypeAttributeSymbol is not null)
            {
                var primaryKeyTypeAttribute = interfaceTypeSymbol
                    ?.GetAttributes()
                    .FirstOrDefault(a =>
                        SymbolEqualityComparer.Default.Equals(
                            a.AttributeClass?.OriginalDefinition,
                            primaryKeyTypeAttributeSymbol
                        )
                    );

                if (primaryKeyTypeAttribute is not null)
                {
                    return ToTypeKeyword(primaryKeyTypeAttribute);
                }
            }
            return "int";
        }

        public static string ResolvePrimaryKeyType(
            GeneratorAttributeSyntaxContext context,
            ISymbol interfaceSymbol
        )
        {
            var primaryKeyAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                "RepositoryGenerator.Library.Attributes.PrimaryKeyTypeIsAttribute`1"
            );

            var primaryKeyattribute = interfaceSymbol
                .GetAttributes()
                .FirstOrDefault(a =>
                    SymbolEqualityComparer.Default.Equals(
                        a.AttributeClass?.OriginalDefinition,
                        primaryKeyAttributeSymbol
                    )
                );

            if (primaryKeyattribute is not null)
            {
                return ToTypeKeyword(primaryKeyattribute);
            }

            return "int";
        }

        private static string ToTypeKeyword(AttributeData primaryKeyattribute)
        {
            var keyType = primaryKeyattribute.AttributeClass?.TypeArguments.FirstOrDefault();

            return keyType?.SpecialType switch
            {
                SpecialType.System_Int32 => "int",
                SpecialType.System_Int64 => "long",
                SpecialType.System_String => "string",
                _ => keyType?.ToDisplayString() ?? "int",
            };
        }
    }
}
