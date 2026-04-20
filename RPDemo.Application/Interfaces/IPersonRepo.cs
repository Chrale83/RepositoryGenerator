using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Application.Interfaces
{
    [DbRepository<Person>]
    public partial interface IPersonRepo { }
}
