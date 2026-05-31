using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Domain.Interfaces
{
    [PrimaryKeyTypeIs<long>]
    [RepositoryFor<Person>]
    public partial interface IPersonRepository { }
}
