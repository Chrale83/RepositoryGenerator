using System.Collections.Immutable;
using System.Linq;
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
            "RepositoryGenerator.Library.Attributes.RepositoryForAttribute`1";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaces = context
                .SyntaxProvider.ForAttributeWithMetadataName(
                    InterfaceExtensionAttribute,
                    static (_, _) => true,
                    (ctx, _) => InterfaceTargetParser.Parse(ctx)
                )
                .Collect();

            var classes = context
                .SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (node, _) => IsClassTarget(node),
                    transform: static (ctx, _) => RepositoryTargetParser.Parse(ctx)
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
                static (ctx, source) => ExecuteDIRegistration(ctx, source)
            );
        }

        private static void ExecuteDIRegistration(
            SourceProductionContext ctx,
            ImmutableArray<ClassToGenerate?> classes
        )
        {
            if (classes.IsDefaultOrEmpty)
            {
                return;
            }

            var source = CodeWriter.WriteDIRegistration(classes);

            if (source is not null)
            {
                ctx.AddSource(source.FileName, source.Code);
            }
        }

        private static void ExecuteClass(
            SourceProductionContext context,
            ImmutableArray<ClassToGenerate?> classes
        )
        {
            foreach (var classToGenerate in classes)
            {
                if (classToGenerate is null)
                {
                    continue;
                }

                var source = CodeWriter.WriteRepositoryClass(classToGenerate);

                context.AddSource(source.FileName, source.Code);
            }
        }

        private static bool IsClassTarget(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax c || c.AttributeLists.Count == 0)
            {
                return false;
            }

            return c.AttributeLists.Any(al =>
                al.Attributes.Any(a => a.Name.ToString().Contains("DbRepositoryFor"))
            );
        }

        private static void ExecuteInterface(
            SourceProductionContext context,
            ImmutableArray<InterfaceToGenerate?> interfaces
        )
        {
            foreach (var interfaceToGenerate in interfaces)
            {
                if (interfaceToGenerate is null)
                {
                    continue;
                }
                var source = CodeWriter.WriteRepositoryInterface(interfaceToGenerate);

                context.AddSource(source.FileName, source.Code);
            }
        }
    }
}
