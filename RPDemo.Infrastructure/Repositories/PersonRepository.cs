using RepositoryGenerator.Library.Attributes;
using RPDemo.Application.Interfaces;
using RPDemo.Domain.Entities;

namespace RPDemo.Infrastructure.Repositories
{
    [DbRepositoryFor<Person, DemoDbContext>]
    public partial class PersonRepository : IPersonRepo { }
}
