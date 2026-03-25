using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPDemo.Application.Interfaces;
using RPDemo.Infrastructure;
using RPDemo.Infrastructure.Repositories;

Console.WriteLine("Hello, World!");

ServiceCollection services = new();

services.AddGeneratedServices();

services.AddDbContext<DemoDbContext>(options => options.UseInMemoryDatabase("Demo"));

services.BuildServiceProvider();
