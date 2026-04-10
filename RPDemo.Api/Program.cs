using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPDemo.Infrastructure;

ServiceCollection services = new();

services.AddGeneratedServices();

services.AddDbContext<DemoDbContext>(options => options.UseInMemoryDatabase("Demo"));

services.BuildServiceProvider();
