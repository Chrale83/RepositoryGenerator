using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryGenerator.Generator.Helpers;
using RepositoryGenerator.Generator.Models;

namespace RepositoryGenerator.Generator
{
    [Generator]
    internal class RPGenerator : IIncrementalGenerator
    {
        private const string InterfaceExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute`1";
        private const string ClassExtensionAttribute =
            "RepositoryGenerator.Library.Attributes.RPClassAttribute`2";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //var interfaces = context
            //    .SyntaxProvider.CreateSyntaxProvider(
            //        predicate: static (node, _) => IsInterfaceTarget(node),
            //        transform: static (ctx, _) => GetInterfaceTarget(ctx)
            //    )
            //    .Collect();

            var interfaces = context
                .SyntaxProvider.ForAttributeWithMetadataName(
                    InterfaceExtensionAttribute,
                    static (_, _) => true,
                    (ctx, _) => GetInterfaceTarget(ctx)
                )
                .Collect();

            var classes = context
                .SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (node, _) => IsClassTarget(node),
                    transform: static (ctx, _) => GetClassTarget(ctx)
                )
                .Collect();

            context.RegisterSourceOutput(
                interfaces,
                static (ctx, source) => ExecuteInterface(ctx, source)
            );

            context.RegisterSourceOutput(
                classes,
                static (ctx, source) => ExecuteClass(ctx, source)
            );

            context.RegisterSourceOutput(
                classes,
                static (ctx, source) => BuildDIRegistration(ctx, source)
            );
        }

        private static InterfaceToGenerate? GetInterfaceTarget(
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

            //Hämta interface metadata

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

        private static void BuildDIRegistration(
            SourceProductionContext ctx,
            ImmutableArray<ClassToGenerate> classes
        )
        {
            if (classes.IsDefaultOrEmpty)
            {
                return;
            }

            var usings = new HashSet<string> { "Microsoft.Extensions.DependencyInjection" };

            foreach (var item in classes)
            {
                usings.Add(item.ClassNamespaceName);
                usings.Add(item.InterfaceUsingNamespace);
            }

            var sb = new StringBuilder();

            foreach (var item in usings.OrderBy(x => x))
                sb.AppendLine($"using {item};");

            sb.AppendLine();
            sb.AppendLine("public static class ServiceCollectionExtensions");
            sb.AppendLine("{");
            sb.AppendLine(
                "    public static IServiceCollection AddGeneratedServices(this IServiceCollection services)"
            );
            sb.AppendLine("    {");

            foreach (var c in classes)
            {
                sb.AppendLine($"        services.AddScoped<{c.InterfaceName}, {c.ClassName}>();");
            }

            sb.AppendLine("        return services;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            ctx.AddSource("AddRepositories.g.cs", sb.ToString());
        }

        #region class stuff
        private static void ExecuteClass(
            SourceProductionContext context,
            ImmutableArray<ClassToGenerate?> classes
        )
        {
            if (classes == null)
            {
                return;
            }

            foreach (var classToGenerate in classes)
            {
                if (classToGenerate is null)
                {
                    continue;
                }

                var source = CodeWriter.WriteRepoClass(classToGenerate);

                context.AddSource(source.FileName, source.Code);
            }
        }

        private static ClassToGenerate? GetClassTarget(GeneratorSyntaxContext context)
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

            var isPartial = classSymbol.DeclaringSyntaxReferences.Any(syntax => syntax.GetSyntax() is BaseTypeDeclarationSyntax declaration && declaration.Modifiers.Any(modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)));

            if (!isPartial)
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
            ImmutableArray<InterfaceToGenerate?> interfaces
        )
        {
            if (interfaces == null)
            {
                return;
            }

            foreach (var interfaceToGenerate in interfaces)
            {
                if (interfaceToGenerate is null)
                {
                    continue;
                }
                var source = CodeWriter.WriteInterface(interfaceToGenerate);

                context.AddSource(source.FileName, source.Code);
            }
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

        //private static InterfaceToGenerate? GetInterfaceTarget(GeneratorSyntaxContext context)
        //{
        //    //Se om den har attributet
        //    var attributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
        //        InterfaceExtensionAttribute
        //    );

        //    if (attributeSymbol == null)
        //    {
        //        return null;
        //    }

        //    //Hämta interface metadata
        //    var interfaceDeclarationSyntax = (InterfaceDeclarationSyntax)context.Node;
        //    var interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(
        //        interfaceDeclarationSyntax
        //    );

        //    if (interfaceSymbol is null)
        //    {
        //        return null;
        //    }

        //    //Hämta attributet

        //    var attribute = interfaceSymbol
        //        .GetAttributes()
        //        .FirstOrDefault(a =>
        //            a.AttributeClass?.OriginalDefinition.ToDisplayString()
        //            == "RepositoryGenerator.Library.Attributes.RPInterfaceAttribute<T>"
        //        );

        //    if (attribute is null)
        //    {
        //        return null;
        //    }

        //    var typeArgument = attribute.AttributeClass?.TypeArguments.FirstOrDefault();
        //    if (typeArgument is null)
        //    {
        //        return null;
        //    }

        //    var argumentName = typeArgument.Name;
        //    var argumentUsingName = typeArgument.ContainingNamespace.ToDisplayString();

        //    var interfaceName = interfaceSymbol.Name;
        //    var interfaceNamespaceName = interfaceSymbol.ContainingNamespace.ToDisplayString();

        //    var interfacedata = new InterfaceToGenerate(
        //        interfaceNamespaceName,
        //        interfaceName,
        //        argumentName,
        //        argumentUsingName
        //    );

        //    return interfacedata;
        //}
        #endregion
    }
}
