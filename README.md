# RepositoryGenerator

A C# source generator that automatically generates the **Repository Pattern** for your Entity Framework Core entities. Reduce boilerplate by letting the generator create your repository interfaces and implementations at compile time.

---

## Features

- Automatically generates repository interfaces with standard CRUD methods
- Automatically generates repository class implementations using EF Core
- Generates a `ServiceCollectionExtensions` class for easy dependency injection registration
- Zero runtime overhead — all code is generated at compile time via Roslyn

---

## Requirements

- .NET project using **Entity Framework Core**
- Entities must follow a primary key convention (see [Primary Key Convention](#primary-key-convention))

---

## Installation

Install the NuGet package:

```bash
dotnet add package RepositoryGenerator
```

Or via the NuGet Package Manager in Visual Studio.

---

## Getting Started

### 1. Define your entity

```csharp
public class Product
{
    public int ProductId { get; set; } // or just "Id"
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### 2. Create your DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
```

### 3. Declare a partial interface with `[RPInterface]`

Mark your repository interface with `[RepositoryForAttribute <TEntity>]`. The generator will fill in the CRUD method signatures.

```csharp
using RepositoryGenerator.Library.Attributes;

[RepositoryForAttribute <Product>]
public partial interface IProductRepository
{
}
```

The generator will produce:

```csharp
public partial interface IProductRepository
{
    Task<Product> GetById(int id);
    Task<IEnumerable<Product>> GetAll();
    Task Add(Product entity);
    Task Update(Product entity);
    Task Delete(Product entity);
}
```

### 4. Declare a partial class with `[RPClass]`

Mark your repository class with `[DbRepositoryForAttribute <TEntity, TDbContext>]`. The class must implement your interface. The generator fills in the method bodies.

```csharp
using RepositoryGenerator.Library.Attributes;

[DbRepositoryForAttribute <Product, AppDbContext>]
public partial class ProductRepository : IProductRepository
{
}
```

The generator will produce a fully implemented class:

```csharp
public partial class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product> GetById(int id)
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
```

### 5. Register services

The generator produces an extension method so you can register all repositories in one call:

```csharp
// Program.cs
builder.Services.AddGeneratedServices();
```

This registers all generated repositories as **scoped** services, e.g.:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

---

## Primary Key Convention

The generator automatically detects the primary key of your entity. It looks for a property named either:

- `{EntityName}Id` — e.g., `ProductId` for a `Product` entity
- `Id`

The property **must be of type `int`**. If no matching property is found, the generator will skip the class.

---

## Generated Methods

| Method | Description |
|---|---|
| `GetById(int id)` | Returns a single entity by its primary key |
| `GetAll()` | Returns all entities in the table |
| `Add(TEntity entity)` | Adds the entity and saves changes |
| `Update(TEntity entity)` | Updates the entity and saves changes |
| `Delete(TEntity entity)` | Removes the entity and saves changes |

---

## Full Example

```csharp
// Entity
public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime CreatedAt { get; set; }
}

// DbContext
public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
}

// Interface (partial — generator fills in the methods)
[RepositoryForAttribute <Order>]
public partial interface IOrderRepository { }

// Class (partial — generator fills in the implementation)
[DbRepositoryForAttribute <Order, ShopDbContext>]
public partial class OrderRepository : IOrderRepository { }

// Program.cs
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddGeneratedRepositories();

// Usage in a controller or service
public class OrderService(IOrderRepository repository)
{
    public Task<IEnumerable<Order>> GetAllOrders() => repository.GetAll();
}
```

---

## Limitations

- Only supports **`int` primary keys**
- One repository per entity — no support for composite keys
- `GetById` returns `null` if no entity is found (no exception thrown)
- All save operations call `SaveChangesAsync` immediately — no unit-of-work batching

---

## License

This project is licensed under the MIT License.
