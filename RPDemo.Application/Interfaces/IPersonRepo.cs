using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Application.Interfaces
{
    [RepositoryFor<Person>]
    public partial interface IPersonRepo { }
}
