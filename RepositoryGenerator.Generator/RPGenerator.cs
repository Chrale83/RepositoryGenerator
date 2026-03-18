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
            var interfaces = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsInterfaceTarget(node),
                transform: static (ctx, _) => GetInterfaceTarget(ctx)
            );

            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsClassTarget(node),
                transform: static (ctx, _) => GetClassTarget(ctx)
            );

            context.RegisterSourceOutput(
                interfaces,
                static (ctx, source) => ExecuteInterface(ctx, source)
            );
        }

        #region Class stuff

        private static ClassToGenerateData? GetClassTarget(GeneratorSyntaxContext context)
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

            //Hämta attributdata

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

            var className = classSymbol.Name;
            var classNamespace = classSymbol.ContainingNamespace.Name;

            var dbContextArgument = attribute.AttributeClass.TypeArguments[1];

            var dbArgumentName = dbContextArgument.Name;
            var dbArgumentUsing = dbContextArgument.ContainingNamespace.ToDisplayString();
            var interfaceType = classSymbol.Interfaces.FirstOrDefault();
            if (interfaceType is null)
            {
                return null;
            }
            var interfaceName = interfaceType.Name;
            var interfaceUsing = interfaceType.ContainingNamespace.ToDisplayString();

            return new ClassToGenerateData(
                classNamespace,
                className,
                entityUsingName,
                entityName,
                dbArgumentUsing,
                dbArgumentName,
                interfaceName,
                interfaceUsing
            );
        }

        private static bool IsClassTarget(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classDeclaration)
            {
                var hasAttributes = classDeclaration.AttributeLists.Count > 0;
                return hasAttributes;
            }
            return false;
        }
        #endregion


        #region Interface stuff
        private static void ExecuteInterface(
            SourceProductionContext context,
            InterfaceToGenerate? interfaceToGenerate
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

        private static InterfaceToGenerate? GetInterfaceTarget(GeneratorSyntaxContext context)
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

            var interfacedata = new InterfaceToGenerate(
                interfaceNamespaceName,
                interfaceName,
                argumentName,
                argumentUsingName
            );

            return interfacedata;
        }
        #endregion
    }
}
