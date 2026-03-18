using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryGenerator.Library.Models;
using System;
using System.Linq;

namespace RepositoryGenerator.Generator
{
    [Generator]
    internal class RPGenerator : IIncrementalGenerator
    {
        private const string InterfaceExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute`1";
        private const string ClassExtensionAttribute = "RepositoryGenerator.Library.Attributes.RPClassAttribute`1"
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaces = context.SyntaxProvider.CreateSyntaxProvider(
                 predicate: static (node, _) => IsInterfaceTarget(node),
                 transform: static (ctx, _) => GetInterfaceTarget(ctx)
             );



        }

        private static bool IsInterfaceTarget(SyntaxNode node)
        {
            if (node is InterfaceDeclarationSyntax)
            {
                return true;
            }
            return false;
        }

        private static InterfaceInfo? GetInterfaceTarget(GeneratorSyntaxContext context)
        {
            //Se om den har attributet
            var attributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
                InterfaceExtensionAttribute
            );

            if (attributeSymbol == null)
            {
                return null;
            }

            //Hämta interface metadata
            var interfaceDeclarationSyntax = (InterfaceDeclarationSyntax)context.Node;
            var interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(
                interfaceDeclarationSyntax
            );

            //Hämta attributet

            var attribute = interfaceSymbol.GetAttributes().FirstOrDefault
        }
    }
}
