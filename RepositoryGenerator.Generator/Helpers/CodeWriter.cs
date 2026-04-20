using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using RepositoryGenerator.Generator.Models;

namespace RepositoryGenerator.Generator.Helpers
{
    internal static class CodeWriter
    {
        public static Source WriteInterface(InterfaceToGenerate interfaceToGenerate)
        {
            var interfaceNamespaceName = interfaceToGenerate.NamespaceName;
            var interfaceName = interfaceToGenerate.InterfaceName;
            var argumentTypeName = interfaceToGenerate.ArgumentName;
            var argumentUsing = interfaceToGenerate.ArgumentUsingName;

            var fileName = $"{interfaceNamespaceName}.{interfaceName}.g.cs";

            var code =
                $@"
using {argumentUsing};

namespace {interfaceNamespaceName}
{{
    public partial interface {interfaceName}
    {{
        Task<{argumentTypeName}> GetById(int id);
        Task<IEnumerable<{argumentTypeName}>> GetAll();
        Task Add({argumentTypeName} entity);
        Task Update({argumentTypeName} entity);
        Task Delete({argumentTypeName} entity);
    }}

}}";
            return new Source(code, fileName);
        }

        public static Source WriteRepoClass(ClassToGenerate classToGenerate)
        {
            var classNamespaceName = classToGenerate.ClassNamespaceName;
            var className = classToGenerate.ClassName;
            var entityName = classToGenerate.EntityName;
            var entityUsingName = classToGenerate.EntityUsingName;
            var dbContextUsingName = classToGenerate.DbContextUsingName;
            var dbCOntextName = classToGenerate.DbContextName;
            var interfaceName = classToGenerate.InterfaceName;
            var interfaceUsingNamespace = classToGenerate.InterfaceUsingNamespace;
            var dbSetName = classToGenerate.DbSetName;
            var primaryKey = classToGenerate.EntityPrimaryKey;

            var fileName = $"{classNamespaceName}.{className}.g.cs";
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(
                $@"
using {entityUsingName};
using {interfaceUsingNamespace};
using {dbContextUsingName};
using Microsoft.EntityFrameworkCore;

namespace {classNamespaceName}
{{
     public partial class {className}({dbCOntextName} context) : {interfaceName}
     {{
         public async Task<{entityName}> GetById(int id)
         {{
                 return await context.{dbSetName}.FirstOrDefaultAsync(x => x.{primaryKey} == id);
         }}
         
         public async Task<IEnumerable<{entityName}>> GetAll()
         {{
                 return await context.{dbSetName}.ToListAsync();
         }}

         public async Task Add({entityName} entity)
         {{
                 context.{dbSetName}.Add(entity);
                 await context.SaveChangesAsync();   
         }}

         public async Task Delete({entityName} entity)
         {{
                context.{dbSetName}.Remove(entity);
                await context.SaveChangesAsync();
         }}

         public async Task Update({entityName} entity)
         {{
                context.{dbSetName}.Update(entity);
                await context.SaveChangesAsync();
         }}
     }}
}}

"
            );

            return new Source(stringBuilder.ToString(), fileName);
        }

        internal static Source? WriteDIRegistration(ImmutableArray<ClassToGenerate?> classes)
        {
            var usings = new HashSet<string> { "Microsoft.Extensions.DependencyInjection" };

            foreach (var item in classes)
            {
                if (item is null)
                {
                    continue;
                }
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
                "    public static IServiceCollection AddGeneratedRepositories(this IServiceCollection services)"
            );
            sb.AppendLine("    {");

            foreach (var c in classes)
            {
                if (c is null)
                {
                    continue;
                }
                sb.AppendLine($"        services.AddScoped<{c.InterfaceName}, {c.ClassName}>();");
            }

            sb.AppendLine("        return services;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new Source(sb.ToString(), "AddRepositories.g.cs");
        }
    }

    public sealed class Source(string code, string fileName)
    {
        public string Code { get; set; } = code;
        public string FileName { get; set; } = fileName;
    }
}
