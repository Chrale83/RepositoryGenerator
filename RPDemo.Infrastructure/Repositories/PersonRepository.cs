using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;
using RPDemo.Domain.Interfaces;

namespace RPDemo.Infrastructure.Repositories
{
    [DbRepositoryFor<Person, DemoDbContext>]
    public partial class PersonRepository : IPersonRepository { }
}
