using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryGenerator.Generator.Models;

namespace RepositoryGenerator.Generator.Helpers
{
    internal static class ClassHelper
    {
        private const string ClassExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.DbRepositoryForAttribute`2";

        internal static ClassToGenerate? GetClassTarget(GeneratorSyntaxContext context)
        {
            //KOlla om den har rätt attribut
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            var attributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                ClassExtensionAttribute
            );

            if (
                context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)
                is not INamedTypeSymbol classSymbol
            )
            {
                return null;
            }

            if (attributeSymbol is null)
            {
                return null;
            }

            if (classSymbol is null)
            {
                return null;
            }

            var attribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(attr =>
                    attr.AttributeClass?.ConstructedFrom.Equals(
                        attributeSymbol,
                        SymbolEqualityComparer.Default
                    ) == true
                );

            if (attribute is null)
            {
                return null;
            }

            var entity = attribute.AttributeClass?.TypeArguments.FirstOrDefault();
            if (entity is null)
            {
                return null;
            }
            var entityName = entity.Name;
            var entityUsingName = entity.ContainingNamespace.ToDisplayString();

            //Get the primarykey for the entity

            var conventionPrimaryKey = $"{entityName}Id";

            var props = entity
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p =>
                    string.Equals(p.Name, conventionPrimaryKey, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
            var entityPrimaryKey = string.Empty;
            if (props.Count == 1 && props[0].Type.SpecialType == SpecialType.System_Int32)
            {
                entityPrimaryKey = props[0].Name;
            }
            else
            {
                return null;
            }

            var className = classSymbol.Name;
            var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();

            var dbContextArgument = attribute.AttributeClass.TypeArguments[1];
            // Need to get the database name for the entity
            // Get the the properties from the DbContext
            // Pick the one that is a propertytype of DbSet and have the selected entity, get the name of the dbset

            var dbSetSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                "Microsoft.EntityFrameworkCore.DbSet`1"
            );

            var propertyName = dbContextArgument
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p =>
                    p.Type is INamedTypeSymbol namedType
                    && SymbolEqualityComparer.Default.Equals(
                        namedType.OriginalDefinition,
                        dbSetSymbol
                    )
                    && SymbolEqualityComparer.Default.Equals(namedType.TypeArguments[0], entity)
                )
                .Select(p => p.Name)
                .FirstOrDefault();

            var dbsetName = propertyName;

            var dbArgumentName = dbContextArgument.Name;
            var dbArgumentUsing = dbContextArgument.ContainingNamespace.ToDisplayString();
            var interfaceType = classSymbol.Interfaces.FirstOrDefault();
            if (interfaceType is null)
            {
                return null;
            }
            var interfaceName = interfaceType.Name;
            var interfaceUsing = interfaceType.ContainingNamespace.ToDisplayString();

            return new ClassToGenerate(
                classNamespace,
                className,
                entityUsingName,
                entityName,
                dbArgumentUsing,
                dbArgumentName,
                interfaceName,
                interfaceUsing,
                entityPrimaryKey,
                dbsetName
            );
        }
    }
}
