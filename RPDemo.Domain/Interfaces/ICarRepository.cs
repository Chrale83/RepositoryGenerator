using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Domain.Interfaces
{
    [RepositoryFor<Car>]
    public partial interface ICarRepository { }
}
