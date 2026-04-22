using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Domain.Interfaces
{
    [PrimaryKeyTypeIs<long>]
    [RepositoryFor<Car>]
    public partial interface ICarRepository { }
}
