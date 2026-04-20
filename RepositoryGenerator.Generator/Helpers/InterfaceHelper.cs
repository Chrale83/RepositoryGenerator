using System.Linq;
using Microsoft.CodeAnalysis;
using RepositoryGenerator.Generator.Models;

namespace RepositoryGenerator.Generator.Helpers
{
    internal static class InterfaceHelper
    {
        internal static InterfaceToGenerate? GetInterfaceTarget(
            GeneratorAttributeSyntaxContext context
        )
        {
            var attributeSymbol = (INamedTypeSymbol)context.TargetSymbol;
            if (attributeSymbol == null)
            {
                return null;
            }

            var interfaceDeclarationSyntax = attributeSymbol
                .DeclaringSyntaxReferences[0]
                .GetSyntax();

            var interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(
                interfaceDeclarationSyntax
            );

            if (interfaceSymbol is null)
            {
                return null;
            }

            var attribute = interfaceSymbol
                .GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.OriginalDefinition.ToDisplayString()
                    == "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute<T>"
                );

            if (attribute is null)
            {
                return null;
            }

            var typeArgument = attribute.AttributeClass?.TypeArguments.FirstOrDefault();
            if (typeArgument is null)
            {
                return null;
            }

            var argumentName = typeArgument.Name;
            var argumentUsingName = typeArgument.ContainingNamespace.ToDisplayString();

            var interfaceName = interfaceSymbol.Name;
            var interfaceNamespaceName = interfaceSymbol.ContainingNamespace.ToDisplayString();

            var interfacedata = new InterfaceToGenerate(
                interfaceNamespaceName,
                interfaceName,
                argumentName,
                argumentUsingName
            );

            return interfacedata;
        }
    }
}
