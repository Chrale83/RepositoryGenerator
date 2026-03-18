using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryGenerator.Library.Models;

namespace RepositoryGenerator.Generator
{
    [Generator]
    internal class RPGenerator : IIncrementalGenerator
    {
        private const string InterfaceExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute`1";
        private const string ClassExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.RPClassAttribute`1";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("Test.g.cs", "// Hello from generator")
            );
            var interfaces = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsInterfaceTarget(node),
                transform: static (ctx, _) => GetInterfaceTarget(ctx)
            );

            context.RegisterSourceOutput(
                interfaces,
                static (ctx, source) => ExecuteInterface(ctx, source)
            );
        }

        private static void ExecuteInterface(
            SourceProductionContext context,
            InterfaceInfo? interfaceToGenerate
        )
        {
            if (interfaceToGenerate == null)
            {
                return;
            }

            var interfaceNamespaceName = interfaceToGenerate.NamespaceName;
            var interfaceName = interfaceToGenerate.InterfaceName;
            var argumentTypeName = interfaceToGenerate.ArgumentName;
            var argumentUsing = interfaceToGenerate.ArgumentUsingName;

            var fileName = $"{interfaceNamespaceName}.{interfaceName}.g.cs";

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(
                $@"
using {argumentUsing};

namespace {interfaceNamespaceName}
{{
    public partial interface {interfaceName}
    {{
        Task<{argumentTypeName}> GetById(int id);
        Task<List<{argumentTypeName}>> GetByIdList(int id);

    }}

}}

"
            );

            context.AddSource(fileName, stringBuilder.ToString());
        }

        private static bool IsInterfaceTarget(SyntaxNode node)
        {
            if (node is InterfaceDeclarationSyntax interfaceDeclaration)
            {
                var hasAttributes = interfaceDeclaration.AttributeLists.Count > 0;
                return hasAttributes;
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

            if (interfaceSymbol is null)
            {
                return null;
            }

            //Hämta attributet

            var attribute = interfaceSymbol
                .GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.OriginalDefinition.ToDisplayString()
                    == "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute<T>"
                );

            //var attribute = interfaceSymbol
            //    .GetAttributes()
            //    .FirstOrDefault(a =>
            //        a.AttributeClass?.ToDisplayString() == InterfaceExtensionAttribute
            //    );

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

            var interfacedata = new InterfaceInfo(
                interfaceNamespaceName,
                interfaceName,
                argumentName,
                argumentUsingName
            );

            return interfacedata;
        }
    }
}
