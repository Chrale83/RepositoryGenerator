# RepositoryGenerator

A C# Source Generator that automatically generates repository classes and interfaces for Entity Framework Core, with built-in dependency injection registration — all at compile time with zero runtime overhead.

---

## Features

- Generates `IRepository`-style interfaces with standard CRUD methods
- Generates concrete repository implementations backed by EF Core
- Auto-registers all repositories with the DI container via a generated extension method
- Supports custom primary key names and types
- Uses `partial` classes and interfaces — you can extend generated code freely
- Zero reflection, zero runtime cost

---

## Installation

Install both packages via NuGet:

```bash
dotnet add package RepositoryGenerator
dotnet add package RepositoryGenerator.Library
```

Or via the NuGet Package Manager in Visual Studio.

---

## Getting Started

### 1. Mark your interface with `[RepositoryFor<T>]`

```csharp
using RepositoryGenerator.Library.Attributes;

[RepositoryFor<Product>]
public partial interface IProductRepository { }
```

The generator will expand this interface to include:

```csharp
Task<Product> GetById(int id);
Task<IEnumerable<Product>> GetAll();
Task Add(Product entity);
Task Update(Product entity);
Task Delete(Product entity);
```

### 2. Mark your repository class with `[DbRepositoryFor<TEntity, TDbContext>]`

```csharp
using RepositoryGenerator.Library.Attributes;

[DbRepositoryFor<Product, AppDbContext>]
public partial class ProductRepository : IProductRepository { }
```

The generator will implement the full repository body using your `AppDbContext` and the matching `DbSet<Product>`.

### 3. Register repositories in your DI container

The generator produces a single extension method that registers all repositories:

```csharp
builder.Services.AddGeneratedRepositories();
```

---

## Primary Key Configuration

By default, the generator looks for a property named `Id` or `{EntityName}Id` of type `int`. You can override this behavior using the following attributes.

### Override the primary key property name

Apply `[PrimaryKeyIs("PropertyName")]` to your repository class:

```csharp
[DbRepositoryFor<Order, AppDbContext>]
[PrimaryKeyIs("OrderNumber")]
public partial class OrderRepository : IOrderRepository { }
```

### Override the primary key type

Apply `[PrimaryKeyTypeIs<TKey>]` to your interface:

```csharp
[RepositoryFor<Order>]
[PrimaryKeyTypeIs<Guid>]
public partial interface IOrderRepository { }
```

Supported key types: `int` (default), `long`, `string`, `Guid`, or any custom type.

---

## Generated Code Example

Given this setup:

```csharp
// Entity
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
}

// Interface
[RepositoryFor<Product>]
public partial interface IProductRepository { }

// Repository class
[DbRepositoryFor<Product, AppDbContext>]
public partial class ProductRepository : IProductRepository { }
```

The generator produces:

```csharp
// IProductRepository.g.cs
public partial interface IProductRepository
{
    Task<Product> GetById(int id);
    Task<IEnumerable<Product>> GetAll();
    Task Add(Product entity);
    Task Update(Product entity);
    Task Delete(Product entity);
}

// ProductRepository.g.cs
public partial class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetById(int id)
        => await context.Products.FirstOrDefaultAsync(x => x.ProductId == id);

    public async Task<IEnumerable<Product>> GetAll()
        => await context.Products.ToListAsync();

    public async Task Add(Product entity)
    {
        context.Products.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(Product entity)
    {
        context.Products.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Product entity)
    {
        context.Products.Remove(entity);
        await context.SaveChangesAsync();
    }
}

// AddRepositories.g.cs
public static class RepositoryGeneratorServiceCollectionExtensions
{
    public static IServiceCollection AddGeneratedRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}
```

---

## Attributes Reference

| Attribute | Target | Description |
|---|---|---|
| `[RepositoryFor<T>]` | Interface | Generates CRUD method signatures for entity `T` |
| `[DbRepositoryFor<T, TDbContext>]` | Class | Generates EF Core repository implementation |
| `[PrimaryKeyIs("name")]` | Class | Overrides the primary key property name |
| `[PrimaryKeyTypeIs<TKey>]` | Interface | Overrides the primary key type |

---

## Requirements

- .NET 6 or later
- C# 10 or later
- Entity Framework Core 6 or later
- Roslyn-compatible compiler (Visual Studio 2022, `dotnet build`, Rider)

---

## Extending Generated Code

Since generated classes and interfaces are `partial`, you can add your own methods without touching the generated file:

```csharp
// Your own file — safe from regeneration
public partial class ProductRepository
{
    public async Task<IEnumerable<Product>> GetByCategory(string category)
        => await context.Products
            .Where(p => p.Category == category)
            .ToListAsync();
}
```

---

## License

MIT
