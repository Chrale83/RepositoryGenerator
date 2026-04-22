using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryGenerator.Generator.Models;

namespace RepositoryGenerator.Generator.Helpers
{
    internal static class RepositoryTargetParser
    {
        private const string ClassExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.DbRepositoryForAttribute`2";

        internal static ClassToGenerate? TryParse(GeneratorSyntaxContext context)
        {
            //Check if correct attribute
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

            if (attributeSymbol is null || classSymbol is null)
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

            var entityPrimaryKey = PrimaryKeyResolver.ResolvePrimaryKeyName(
                context,
                entity,
                classSymbol
            );

            if (entityPrimaryKey is null)
            {
                return null;
            }

            var className = classSymbol.Name;
            var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();

            if (attribute.AttributeClass is null)
            {
                return null;
            }

            var dbContextArgument = attribute.AttributeClass.TypeArguments[1];

            var dbSetSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                "Microsoft.EntityFrameworkCore.DbSet`1"
            );

            var dbsetName = dbContextArgument
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

            var interfaceType = classSymbol.Interfaces.FirstOrDefault();

            if (interfaceType is null)
            {
                return null;
            }

            var entityPrimaryKeyType = PrimaryKeyResolver.ResolvePrimaryKeyType(
                context,
                interfaceType
            );

            var dbArgumentName = dbContextArgument.Name;
            var dbArgumentUsing = dbContextArgument.ContainingNamespace.ToDisplayString();
            var interfaceName = interfaceType.Name;
            var interfaceUsing = interfaceType.ContainingNamespace.ToDisplayString();

            var dbData = new DbData(dbsetName, dbArgumentName, dbArgumentUsing);
            var entityData = new EntityData(
                entityUsingName,
                entityName,
                entityPrimaryKey,
                entityPrimaryKeyType
            );
            return new ClassToGenerate(
                classNamespace,
                className,
                interfaceName,
                interfaceUsing,
                entityData,
                dbData
            );
        }
    }
}
