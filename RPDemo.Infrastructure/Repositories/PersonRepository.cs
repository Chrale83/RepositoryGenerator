using RepositoryGenerator.Library.Attributes;
using RPDemo.Application.Interfaces;
using RPDemo.Domain.Entities;

namespace RPDemo.Infrastructure.Repositories
{
    [RPClass<Person, DemoDbContext>]
    public partial class PersonRepository : IPersonRepo { }
}
